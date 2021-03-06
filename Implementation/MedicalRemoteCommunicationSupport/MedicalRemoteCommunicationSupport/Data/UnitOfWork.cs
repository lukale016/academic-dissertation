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

    public UnitOfWork(ConnectionMultiplexer redis, 
        MongoClient mongo, 
        ILogger<UnitOfWork> logger, 
        IOptions<DbSettings> dbConfig)
    {
        this.redis = redis;
        redis.ErrorMessage += _redis_ErrorMessage;
        this.mongo = mongo;
        mongoDb = mongo.GetDatabase(dbConfig.Value.MongoDefaultDb);
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
    private IAppointmentRepository appointmentRepository;
    private IMessageRepository messageRepository;

    public IKeyGeneratorService KeyGenerator => keyGenerator ??= new KeyGeneratorService(redis, loggerFactory.CreateLogger<KeyGeneratorService>());
    public IDoctorRepository DoctorRepository => doctorRepository ??= new DoctorRepository(this, mongoDb, redis);
    public IPatientRepository PatientRepository  => patientRepository ??= new PatientRepository(this, mongoDb, redis);
    public ITopicRepostiory TopicRepostiory => topicRepostiory ??= new TopicRepository(this, mongoDb, redis);
    public ICommentRepository CommentRepository => commentRepository ??= new CommentRepository(this, redis);
    public IAppointmentRepository AppointmentRepository => appointmentRepository ??= new AppointmentRepository(this, mongoDb, redis);
    public IMessageRepository MessageRepository => messageRepository ??= new MessageRepository(this, redis);

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
