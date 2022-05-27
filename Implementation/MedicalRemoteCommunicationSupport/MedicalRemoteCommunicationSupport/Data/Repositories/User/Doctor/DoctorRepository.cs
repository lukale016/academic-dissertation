using MongoDB.Driver;
using StackExchange.Redis;
using System.Text.Json;

namespace MedicalRemoteCommunicationSupport.Data.Repositories;

public class DoctorRepository : UserRepository<Doctor>, IDoctorRepository
{
    private IConnectionMultiplexer redis;

    public DoctorRepository(IMongoDatabase mongoDb, IConnectionMultiplexer redis): base(mongoDb)
    {
        this.redis = redis;
    }

    public override async Task<Doctor> GetUser(string username)
    {
        Doctor doctor = await base.GetUser(username);

        IDatabase db = redis.GetDatabase();
        doctor.Requests = (await db.ListRangeAsync(doctor.RequestListKey))
                                   .Select(rv => rv.ToString());
        doctor.Patients = (await db.ListRangeAsync(doctor.PatientListKey))
                                   .Select(rv => );

        return doctor;
    }

    public Task<Doctor> UpdateDoctor(Doctor doctor)
    {
        throw new NotImplementedException();
    }
}
