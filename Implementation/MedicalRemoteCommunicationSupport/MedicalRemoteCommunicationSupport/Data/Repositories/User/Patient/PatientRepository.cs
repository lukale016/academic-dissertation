using MedicalRemoteCommunicationSupport.Filtering;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Text.Json;

namespace MedicalRemoteCommunicationSupport.Data.Repositories;

public class PatientRepository : UserRepository<Patient>, IPatientRepository
{
    private readonly IConnectionMultiplexer redis;
    private IMongoCollection<Topic> topics;
    private readonly UnitOfWork unitOfWork;

    public PatientRepository(UnitOfWork unit, IMongoDatabase mongoDb, IConnectionMultiplexer redis): base(mongoDb)
    {
        this.unitOfWork = unit;
        this.redis = redis;
        this.topics = mongoDb.GetCollection<Topic>(CollectionConstants.Topics);
    }

    public override async Task<Patient> GetUser(string username)
    {
        Patient patient = await base.GetUser(username);

        IDatabase db = redis.GetDatabase();
        List<int> createdIds = (await db.ListRangeAsync(patient.CreatedTopicsKey))
                                    .Select(rv => int.Parse(rv.ToString())).ToList();
        patient.CreatedTopics = (await topics.FindAsync(Builders<Topic>.Filter.Empty))
                                        .ToList().Where(t => createdIds.Contains(t.Id)).ToList();
        patient.SentRequests = (await db.ListRangeAsync(patient.SentRequestsListKey)).Select(rv => rv.ToString());
        patient.MyDoctors = (await db.ListRangeAsync(patient.MyDoctorsListKey)).Select(rv => JsonSerializer.Deserialize<MyConnection>(rv));
        return patient;
    }

    public Task<IEnumerable<Patient>> Search(PatientCriteria criteria)
    {
        return unitOfWork.ReturnMongoFiltrator<Patient, PatientCriteria>().Search(criteria);
    }

    public Task<Patient> UpdatePatient(Patient patient)
    {
        throw new NotImplementedException();
    }

    public async Task RegisterTopicForUser(string username, int id)
    {
        if(string.IsNullOrWhiteSpace(username) || id <= 0)
        {
            throw new ResponseException(StatusCodes.Status400BadRequest, "Parameters not set");
        }

        Patient patient = await GetUser(username);

        await redis.GetDatabase().ListLeftPushAsync(patient.CreatedTopicsKey, id);
    }

    public async Task UnregisterTopicForUser(string username, int id)
    {
        if (string.IsNullOrWhiteSpace(username) || id <= 0)
        {
            throw new ResponseException(StatusCodes.Status400BadRequest, "Parameters not set");
        }

        Patient patient = await GetUser(username);

        await redis.GetDatabase().ListRemoveAsync(patient.CreatedTopicsKey, id);
    }
}
