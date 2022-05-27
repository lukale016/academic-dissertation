using StackExchange.Redis;

namespace MedicalRemoteCommunicationSupport.Services;

public class KeyGeneratorService : IKeyGeneratorService
{
    private ConnectionMultiplexer redis;
    private ILogger<KeyGeneratorService> logger;

    public KeyGeneratorService(ConnectionMultiplexer redis, ILogger<KeyGeneratorService> logger)
    {
        this.redis = redis;
        this.logger = logger;
    }

    public async Task<int> NextInSequence<T>()
    {
        IDatabase db = redis.GetDatabase();
        string sequenceKey = $"IdGenSequence:{nameof(T)}";
        if(db.KeyExists(sequenceKey))
        {
            int id;
            try
            {
                (await db.StringGetAsync(sequenceKey)).TryParse(out id);
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                throw new ResponseException(500, ex.Message);
            }
            await db.StringSetAsync(sequenceKey, id + 1);
            return id;
        }
        await db.StringSetAsync(sequenceKey, 1);
        return 1;
    }
}
