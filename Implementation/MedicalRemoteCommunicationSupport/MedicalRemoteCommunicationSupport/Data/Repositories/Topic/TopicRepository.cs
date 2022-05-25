using MedicalRemoteCommunicationSupport.Services;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Text.Json;

namespace MedicalRemoteCommunicationSupport.Data.Repositories;

public class TopicRepository : ITopicRepostiory
{
    private IMongoDatabase mongo;
    private IMongoCollection<Topic> topics;
    private IKeyBuilderAndGeneratorService keyGenerator;
    private IConnectionMultiplexer redis;
    private readonly UnitOfWork unitOfWork;

    public TopicRepository(UnitOfWork unit, IMongoDatabase mongo, IConnectionMultiplexer redis, IKeyBuilderAndGeneratorService keyGenerator)
    {
        this.unitOfWork = unit;
        this.mongo = mongo;
        this.redis = redis;
        this.topics = mongo.GetCollection<Topic>(CollectionConstants.Topics);
        this.keyGenerator = keyGenerator;
    }

    public async Task<Topic> AddTopic(Topic topic)
    {
        if(topic is null || string.IsNullOrWhiteSpace(topic.Title))
        {
            throw new ResponseException(StatusCodes.Status400BadRequest, "Parameters not set");
        }

        try
        {
            await unitOfWork.PatientRepository.GetUser(topic.Owner);
        }
        catch(ResponseException ex)
        {
            if(ex.Status != StatusCodes.Status404NotFound && ex.Status != StatusCodes.Status500InternalServerError)
            {
                throw;
            }
            await unitOfWork.DoctorRepository.GetUser(topic.Owner);
        }

        topic.Id = await keyGenerator.NextInSequence(SequenceConstants.TopicKey);
        await topics.InsertOneAsync(topic);
        return topic;
    }

    public async Task<int> DeleteTopic(int id)
    {
        if (id <= 0)
        {
            throw new ResponseException(StatusCodes.Status400BadRequest, "Parameters not set.");
        }

        var filter = Builders<Topic>.Filter.Eq(nameof(Topic.Id), id);
        Topic topic = null;
        try
        {
            topic = (await topics.FindAsync(filter)).SingleOrDefault();
        }
        catch(Exception ex)
        {
            throw new ResponseException(StatusCodes.Status500InternalServerError, ex.Message);
        }

        if(topic is null)
        {
            throw new ResponseException(StatusCodes.Status404NotFound, "Topic not found");
        }

        await redis.GetDatabase().KeyDeleteAsync(topic.CommentsKey);
        await topics.DeleteOneAsync(filter);
        return id;
    }

    public async Task<IEnumerable<Topic>> GetAllTopics()
    {
        return (await topics.FindAsync(Builders<Topic>.Filter.Empty)).ToList();
    }

    public async Task<Topic> GetTopic(int id)
    {
        if (id <= 0)
        {
            throw new ResponseException(StatusCodes.Status400BadRequest, "Parameters not set.");
        }

        Topic topic = null;
        try
        {
            var filter = Builders<Topic>.Filter.Eq(nameof(Topic.Id), id);
            topic = (await topics.FindAsync(filter)).SingleOrDefault();
        }
        catch(Exception ex)
        {
            throw new ResponseException(StatusCodes.Status500InternalServerError, ex.Message);
        }

        if(topic is null)
        {
            throw new ResponseException(StatusCodes.Status404NotFound, "Topic not found");
        }

        topic.Comments = (await redis.GetDatabase().ListRangeAsync(topic.CommentsKey))
            .Select(rv => JsonSerializer.Deserialize<Comment>(rv.ToString())).ToList();

        return topic;
    }

    public Task<Topic> UpdateTopic(Topic topic)
    {
        throw new NotImplementedException();
    }
}
