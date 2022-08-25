using MedicalRemoteCommunicationSupport.Filtering;
using MedicalRemoteCommunicationSupport.Helpers;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Text.Json;

namespace MedicalRemoteCommunicationSupport.Data.Repositories;

public class DoctorRepository : UserRepository<Doctor>, IDoctorRepository
{
    private IConnectionMultiplexer redis;
    private readonly UnitOfWork unitOfWork;
    private IFilterHelper filter;

    public DoctorRepository(UnitOfWork unit, IMongoDatabase mongoDb, IConnectionMultiplexer redis, IFilterHelper filter): base(mongoDb)
    {
        this.unitOfWork = unit;
        this.redis = redis;
        this.filter = filter;
    }

    public override async Task<Doctor> GetUser(string username)
    {
        Doctor doctor = await base.GetUser(username);

        IDatabase db = redis.GetDatabase();
        doctor.Requests = (await db.ListRangeAsync(doctor.RequestListKey))
                                   .Select(rv => JsonSerializer.Deserialize<DoctorRequestDto>(rv.ToString()));
        doctor.Patients = (await db.ListRangeAsync(doctor.PatientListKey)).Select(rv => JsonSerializer.Deserialize<MyConnection>(rv));

        return doctor;
    }

    public async Task<IEnumerable<Doctor>> Search(DoctorCriteria criteria)
    {
        return await filter.ReturnMongoFiltrator<Doctor, DoctorCriteria>().Search(criteria);
    }

    public Task<Doctor> UpdateDoctor(Doctor doctor)
    {
        throw new NotImplementedException();
    }
}
