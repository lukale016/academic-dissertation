using MedicalRemoteCommunicationSupport.Data.Repositories;
using MedicalRemoteCommunicationSupport.Helpers;
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
    private IMongoDatabase mongoDb;
    private ILoggerFactory loggerFactory;
    private readonly IFilterHelper filterHelper;

    public UnitOfWork(ConnectionMultiplexer redis,
        MongoClient mongo,
        ILogger<UnitOfWork> logger,
        IOptions<DbSettings> dbConfig,
        IFilterHelper filterHelper)
    {
        this.redis = redis;
        redis.ErrorMessage += _redis_ErrorMessage;
        mongoDb = mongo.GetDatabase(dbConfig.Value.MongoDefaultDb);
        this.logger = logger;
        loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });
        this.filterHelper = filterHelper;
    }

    private void _redis_ErrorMessage(object? sender, RedisErrorEventArgs e)
    {
        logger.LogError(e.Message);
    }

    private IKeyGeneratorService keyGenerator;
    private IDoctorRepository doctorRepository;
    private IPatientRepository patientRepository;
    private ITopicRepostiory topicRepository;
    private ICommentRepository commentRepository;
    private IAppointmentRepository appointmentRepository;
    private IMessageRepository messageRepository;

    public IKeyGeneratorService KeyGenerator => keyGenerator ??= new KeyGeneratorService(redis, loggerFactory.CreateLogger<KeyGeneratorService>());
    public IDoctorRepository DoctorRepository => doctorRepository ??= new DoctorRepository(this, mongoDb, redis, filterHelper);
    public IPatientRepository PatientRepository  => patientRepository ??= new PatientRepository(this, mongoDb, redis, filterHelper);
    public ITopicRepostiory TopicRepository => topicRepository ??= new TopicRepository(this, mongoDb, redis);
    public ICommentRepository CommentRepository => commentRepository ??= new CommentRepository(this, redis);
    public IAppointmentRepository AppointmentRepository => appointmentRepository ??= new AppointmentRepository(this, mongoDb, redis, filterHelper);
    public IMessageRepository MessageRepository => messageRepository ??= new MessageRepository(this, redis);
}
