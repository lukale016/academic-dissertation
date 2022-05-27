using MedicalRemoteCommunicationSupport.Services;
using MongoDB.Driver;
using StackExchange.Redis;

namespace MedicalRemoteCommunicationSupport.Data.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly UnitOfWork unitOfWork;
    private ConnectionMultiplexer redis;
    private IMongoDatabase mongo;
    private IMongoCollection<Appointment> appointments;

    public AppointmentRepository(UnitOfWork unit, IMongoDatabase mongo, ConnectionMultiplexer redis)
    {
        this.unitOfWork = unit;
        this.mongo = mongo;
        this.redis = redis;
        appointments = mongo.GetCollection<Appointment>(CollectionConstants.Appointments);
    }

    public async Task<Dictionary<string, IEnumerable<Appointment>>> GetAppointments(string username)
    {
        if(string.IsNullOrWhiteSpace(username))
        {
            throw new ResponseException(StatusCodes.Status400BadRequest, "Parameters not set");
        }

        UserBase user = null;
        try
        {
            user = await unitOfWork.PatientRepository.GetUser(username);
        }
        catch(ResponseException ex)
        {
            if(ex.Status != StatusCodes.Status404NotFound && ex.Status != StatusCodes.Status500InternalServerError)
            {
                throw;
            }
            user = await unitOfWork.DoctorRepository.GetUser(username);
        }

        var result = new Dictionary<string, IEnumerable<Appointment>>();
        IDatabase db = redis.GetDatabase();
        List<string> dates = (await db.ListRangeAsync(user.AppointmentDatesListKey))
                                      .Select(rv => rv.ToString())
                                      .ToList();
        
        foreach (string date in dates)
        {
            result.Add(date, db.ListRange(RedisHelperService.BuildAppointmentKey(username, DateTime.Parse(date)))
                               .Select(rv => int.Parse(rv))
                               .Select(id => appointments.Find(Builders<Appointment>.Filter.Eq(nameof(Appointment.Id), id)).SingleOrDefault())
                               .Where(ap => ap is not null));
        }

        return result;
    }

    public async Task<Appointment> RegisterAppointment(AppointmentPostDto dto)
    {
        if (dto is null || string.IsNullOrWhiteSpace(dto.Doctor) || string.IsNullOrWhiteSpace(dto.Patient))
        {
            throw new ResponseException(StatusCodes.Status400BadRequest, "Parameters not set");
        }

        Appointment appointment = new Appointment(dto);
        appointment.Id = await unitOfWork.KeyGenerator.NextInSequence<Appointment>();

        appointment.Patient = await unitOfWork.PatientRepository.GetUser(dto.Patient);
        appointment.Doctor = await unitOfWork.DoctorRepository.GetUser(dto.Doctor);
        await appointments.InsertOneAsync(appointment);

        IDatabase db = redis.GetDatabase();
        await db.ListLeftPushAsync(RedisHelperService.BuildAppointmentKey(appointment.PatientRef, dto.Date.Date), appointment.Id);
        await db.ListLeftPushAsync(RedisHelperService.BuildAppointmentKey(appointment.DoctorRef, dto.Date.Date), appointment.Id);
        await db.ListLeftPushAsync(appointment.Patient.AppointmentDatesListKey, dto.Date.Date.ToString());
        await db.ListLeftPushAsync(appointment.Doctor.AppointmentDatesListKey, dto.Date.Date.ToString());

        return appointment;
    }

    public async Task<int> DeleteAppointment(int id)
    {
        if (id <= 0)
        {
            throw new ResponseException(StatusCodes.Status400BadRequest, "Parameters not set");
        }

        var filter = Builders<Appointment>.Filter.Eq(nameof(Appointment.Id), id);
        Appointment appointment = await appointments.FindOneAndDeleteAsync(filter);

        if(appointment is null)
        {
            throw new ResponseException(StatusCodes.Status404NotFound, "Appointment not found");
        }

        IDatabase db = redis.GetDatabase();
        await db.ListRemoveAsync(RedisHelperService.BuildAppointmentKey(appointment.PatientRef, appointment.ScheduledDate.Date), appointment.Id);
        await db.ListRemoveAsync(RedisHelperService.BuildAppointmentKey(appointment.DoctorRef, appointment.ScheduledDate.Date), appointment.Id);
        await db.ListRemoveAsync(appointment.Patient.AppointmentDatesListKey, appointment.ScheduledDate.Date.ToString());
        await db.ListRemoveAsync(appointment.Doctor.AppointmentDatesListKey, appointment.ScheduledDate.Date.ToString());

        return id;
    }
}
