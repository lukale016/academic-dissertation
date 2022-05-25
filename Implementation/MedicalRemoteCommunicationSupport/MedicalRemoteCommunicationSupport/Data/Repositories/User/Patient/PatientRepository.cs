using MongoDB.Driver;
using StackExchange.Redis;

namespace MedicalRemoteCommunicationSupport.Data.Repositories;

public class PatientRepository : UserRepository<Patient>, IPatientRepository
{
    private readonly IConnectionMultiplexer redis;
    private IMongoCollection<Topic> topics;

    public PatientRepository(IMongoDatabase mongoDb, IConnectionMultiplexer redis): base(mongoDb)
    {
        this.redis = redis;
        topics = mongoDb.GetCollection<Topic>(CollectionConstants.Topics);
    }

    public override async Task<Patient> GetUser(string username)
    {
        Patient patient = await base.GetUser(username);

        IDatabase db = redis.GetDatabase();
        List<int> followedIds = (await db.ListRangeAsync(patient.FollowedTopicsKey))
                                    .Select(rv => int.Parse(rv.ToString())).ToList();
        patient.FollowedTopics = (await topics.FindAsync(Builders<Topic>.Filter.Empty))
                                        .ToList().Where(t => followedIds.Contains(t.Id)).ToList();

        List<int> createdIds = (await db.ListRangeAsync(patient.CreatedTopicsKey))
                                    .Select(rv => int.Parse(rv.ToString())).ToList();
        patient.CreatedTopics = (await topics.FindAsync(Builders<Topic>.Filter.Empty))
                                        .ToList().Where(t => createdIds.Contains(t.Id)).ToList();

        return patient;
    }

    public Task<Patient> UpdatePatient(Patient patient)
    {
        throw new NotImplementedException();
    }
}
