using MedicalRemoteCommunicationSupport.Data.Repositories;
using MedicalRemoteCommunicationSupport.Filtering;
using MedicalRemoteCommunicationSupport.Services;
using MedicalRemoteCommunicationSupport.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StackExchange.Redis;

namespace MedicalRemoteCommunicationSupport.Data;
public class UnitOfWork
{
    private ConnectionMultiplexer redis;
    private ILogger<UnitOfWork> logger;
    private readonly MongoClient mongo;
    private IMongoDatabase mongoDb;
    private ILoggerFactory loggerFactory;

    public UnitOfWork(ILogger<UnitOfWork> logger, IOptions<DbSettings> config)
    {
        redis = ConnectionMultiplexer.Connect(config.Value.RedisConnectionUrl);
        redis.ErrorMessage += _redis_ErrorMessage;
        mongo = new MongoClient(config.Value.MongoConnectionUrl);
        mongoDb = mongo.GetDatabase(config.Value.MongoDefaultDb);
        this.logger = logger;
        loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });
    }

    private void _redis_ErrorMessage(object? sender, RedisErrorEventArgs e)
    {
        logger.LogError(e.Message);
    }

    private IKeyGeneratorService keyGenerator;
    private IDoctorRepository doctorRepository;
    private IPatientRepository patientRepository;
    private ITopicRepostiory topicRepostiory;
    private ICommentRepository commentRepository;

    public IKeyGeneratorService KeyGenerator => keyGenerator ??= new KeyGeneratorService(redis, loggerFactory.CreateLogger<KeyGeneratorService>());
    public IDoctorRepository DoctorRepository => doctorRepository ??= new DoctorRepository(mongoDb, redis);
    public IPatientRepository PatientRepository  => patientRepository ??= new PatientRepository(mongoDb, redis);
    public ITopicRepostiory TopicRepostiory => topicRepostiory ??= new TopicRepository(this, mongoDb, redis);
    public ICommentRepository CommentRepository => commentRepository ??= new CommentRepository(this, redis);

    /// <summary>
    /// Used for dynamic search
    /// </summary>
    /// <typeparam name="T">Mongo model</typeparam>
    /// <typeparam name="Q">ICreteria<T></typeparam>
    /// <returns></returns>
    public IMongoFilter<T, Q> ReturnMongoFiltrator<T, Q>() where Q: ICriteria<T>
    {
        return new MongoFilter<T, Q>(mongoDb);
    }
}
