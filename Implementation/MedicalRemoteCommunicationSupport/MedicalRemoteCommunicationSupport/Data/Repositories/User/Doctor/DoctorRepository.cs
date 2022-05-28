using MedicalRemoteCommunicationSupport.Filtering;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Text.Json;

namespace MedicalRemoteCommunicationSupport.Data.Repositories;

public class DoctorRepository : UserRepository<Doctor>, IDoctorRepository
{
    private IConnectionMultiplexer redis;
    private readonly UnitOfWork unitOfWork;

    public DoctorRepository(UnitOfWork unit, IMongoDatabase mongoDb, IConnectionMultiplexer redis): base(mongoDb)
    {
        this.unitOfWork = unit;
        this.redis = redis;
    }

    public override async Task<Doctor> GetUser(string username)
    {
        Doctor doctor = await base.GetUser(username);

        IDatabase db = redis.GetDatabase();
        doctor.Requests = (await db.ListRangeAsync(doctor.RequestListKey))
                                   .Select(rv => JsonSerializer.Deserialize<RequestDto>(rv.ToString()));

        return doctor;
    }

    public async Task<IEnumerable<Doctor>> Search(DoctorCriteria criteria)
    {
        return await unitOfWork.ReturnMongoFiltrator<Doctor, DoctorCriteria>().Search(criteria);
    }

    public Task<Doctor> UpdateDoctor(Doctor doctor)
    {
        throw new NotImplementedException();
    }
}
