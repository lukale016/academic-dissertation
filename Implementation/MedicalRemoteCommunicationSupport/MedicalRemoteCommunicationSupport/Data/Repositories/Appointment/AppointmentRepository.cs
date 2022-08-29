using System.Text;
using Ardalis.GuardClauses;
using MedicalRemoteCommunicationSupport.Filtering;
using MedicalRemoteCommunicationSupport.Helpers;
using MedicalRemoteCommunicationSupport.Services;
using MongoDB.Driver;
using StackExchange.Redis;

namespace MedicalRemoteCommunicationSupport.Data.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly UnitOfWork unitOfWork;
    private ConnectionMultiplexer redis;
    private IMongoCollection<Appointment> appointments;
    private IFilterHelper filter;

    public AppointmentRepository(UnitOfWork unit, IMongoDatabase mongo, ConnectionMultiplexer redis, IFilterHelper filter)
    {
        this.unitOfWork = unit;
        this.redis = redis;
        appointments = mongo.GetCollection<Appointment>(CollectionConstants.Appointments);
        this.filter = filter;
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
                               .Where(ap => ap is not null)
                               .OrderBy(ap => ap.ScheduledTime));
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
        if (await IsConflicted(appointment))
        {
            throw new ResponseException(StatusCodes.Status409Conflict, "Time slot for appointment is not available.");
        }
        
        await appointments.InsertOneAsync(appointment);
        
        var appointmentDateString = dto.ScheduledTime.ToShortDateString();
        IDatabase db = redis.GetDatabase();
        await db.ListLeftPushAsync(RedisHelperService.BuildAppointmentKey(appointment.PatientRef, dto.ScheduledTime.Date), appointment.Id);
        await db.ListLeftPushAsync(RedisHelperService.BuildAppointmentKey(appointment.DoctorRef, dto.ScheduledTime.Date), appointment.Id);
        
        await db.ListRemoveAsync(appointment.Patient.AppointmentDatesListKey, appointmentDateString);
        await db.ListRemoveAsync(appointment.Doctor.AppointmentDatesListKey, appointmentDateString);
        
        await db.ListLeftPushAsync(appointment.Patient.AppointmentDatesListKey, appointmentDateString);
        await db.ListLeftPushAsync(appointment.Doctor.AppointmentDatesListKey, appointmentDateString);

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

        var appointmentDateString = appointment.ScheduledTime.ToShortDateString();
        IDatabase db = redis.GetDatabase();
        await db.ListRemoveAsync(RedisHelperService.BuildAppointmentKey(appointment.PatientRef, appointment.ScheduledTime.Date), appointment.Id);
        await db.ListRemoveAsync(RedisHelperService.BuildAppointmentKey(appointment.DoctorRef, appointment.ScheduledTime.Date), appointment.Id);
        if(!(await db.ListRangeAsync(RedisHelperService.BuildAppointmentKey(appointment.PatientRef, appointment.ScheduledTime.Date))).Any())
            await db.ListRemoveAsync(appointment.Patient.AppointmentDatesListKey, appointmentDateString);
        if(!(await db.ListRangeAsync(RedisHelperService.BuildAppointmentKey(appointment.DoctorRef, appointment.ScheduledTime.Date))).Any())
            await db.ListRemoveAsync(appointment.Doctor.AppointmentDatesListKey, appointmentDateString);

        return id;
    }

    public async Task<Dictionary<string, IEnumerable<Appointment>>> Search(string username, AppointmentListCriteria? criteria)
    {
        Guard.Against.NullOrEmpty(username, nameof(username)); 
        criteria ??= new AppointmentListCriteria();

        var allAppointments = await GetAppointments(username);
        var result = new Dictionary<string, IEnumerable<Appointment>>();

        if (criteria.ScheduledTime != default)
        {
            IEnumerable<Appointment> appointments;
            if (allAppointments.TryGetValue(criteria.ScheduledTime, out appointments))
            {
                var appointmentsForDate = filter.ReturnListFiltrator<Appointment, AppointmentListCriteria>()
                    .Search(appointments, criteria);
                if(appointmentsForDate.Any())
                    result.Add(criteria.ScheduledTime, appointmentsForDate.OrderBy(ap => ap.ScheduledTime));
                return result;
            }
            else
            {
                return new Dictionary<string, IEnumerable<Appointment>>();
            }
        }

        foreach(var pair in allAppointments)
        {
            var appointmentsForDate = filter.ReturnListFiltrator<Appointment, AppointmentListCriteria>()
                .Search(pair.Value, criteria);
            if(appointmentsForDate.Any())
                result.Add(pair.Key, appointmentsForDate);
        }

        return result;
    }
    
    public async Task<IEnumerable<string>> OccupiedTimeSlots(string username, string scheduledTime)
    {
        var allDoctorAppointmentsForDate = await Search(username,
            new AppointmentListCriteria { ScheduledTime = scheduledTime });

        List<string> result = new();
        try
        {
            var appointments = allDoctorAppointmentsForDate.Select(pair => pair.Value).SingleOrDefault();
            if (appointments is null)
            {
                throw new ResponseException(StatusCodes.Status400BadRequest, "Impossible date");
            }

            appointments = appointments.OrderBy(a => a.ScheduledTime);
            var first = appointments.FirstOrDefault();
            
            if (first is null)
                return result;
            
            StringBuilder entry = new();
            entry.Append($"{first.ScheduledTime.ToLocalTime().ToString("HH:mm")} - ");
            DateTime lowBorder = first.ScheduledTime.ToLocalTime().AddMinutes(first.LengthInMins);
            foreach (var appointment in appointments.Skip(1))
            {
                var localTime = appointment.ScheduledTime.ToLocalTime();
                if (localTime == lowBorder)
                {
                    lowBorder = localTime.AddMinutes(appointment.LengthInMins);
                    continue;
                }

                entry.Append(lowBorder.ToString("HH:mm"));
                result.Add(entry.ToString());
                entry.Clear();
                entry.Append($"{localTime.ToString("HH:mm")} - ");
                lowBorder = localTime.AddMinutes(appointment.LengthInMins);
            }

            entry.Append(lowBorder.ToString("HH:mm"));
            result.Add(entry.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new ResponseException(StatusCodes.Status500InternalServerError, e.Message);
        }

        return result;
    }

    #region Private methods

    private async Task<bool> IsConflicted(Appointment appointment)
    {
        var allDoctorAppointmentsForDate = await Search(appointment.DoctorRef,
            new AppointmentListCriteria { ScheduledTime = appointment.ScheduledTime.ToShortDateString() });

        try
        {
            var appointments = allDoctorAppointmentsForDate.Select(pair => pair.Value).SingleOrDefault();
            
            if (appointments is null)
                return false;
            
            foreach (var scheduled in appointments)
            {
                if (IsInTimeFrame(scheduled, appointment))
                    return true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new ResponseException(StatusCodes.Status500InternalServerError, e.Message);
        }

        return false;
    }

    private bool IsInTimeFrame(Appointment scheduled, Appointment newAppointment)
    {
        DateTime scheduledDateTime = scheduled.ScheduledTime.ToUniversalTime();
        DateTime newAppointmentDateTime = newAppointment.ScheduledTime.ToUniversalTime();
        if (scheduledDateTime == newAppointmentDateTime)
            return true;
        if (scheduledDateTime < newAppointmentDateTime
            && scheduledDateTime.AddMinutes(scheduled.LengthInMins) > newAppointmentDateTime)
            return true;
        if (newAppointmentDateTime < scheduledDateTime
            && newAppointmentDateTime.AddMinutes(newAppointment.LengthInMins) > scheduledDateTime)
            return true;
        return false;
    }

    #endregion
}
