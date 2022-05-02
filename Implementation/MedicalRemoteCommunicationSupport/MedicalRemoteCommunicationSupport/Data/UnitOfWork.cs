using MedicalRemoteCommunicationSupport.Data.Repositories;
using MedicalRemoteCommunicationSupport.Services.KeyBuilderAndGenerator;
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
        //redis = ConnectionMultiplexer.Connect(config.Value.RedisConnectionUrl);
        //redis.ErrorMessage += _redis_ErrorMessage;
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

    private IKeyBuilderAndGeneratorService keyBuilder;
    private IDoctorRepository doctorRepository;
    private IPatientRepository patientRepository;

    public IKeyBuilderAndGeneratorService KeyBuilder { get => keyBuilder ??= new KeyBuilderAndGeneratorService(redis, loggerFactory.CreateLogger<KeyBuilderAndGeneratorService>()); }
    public IDoctorRepository DoctorRepository { get => doctorRepository ??= new DoctorRepository(mongoDb); }
    public IPatientRepository PatientRepository { get => patientRepository ??= new PatientRepository(mongoDb); }
}
