using StackExchange.Redis;

namespace MedicalRemoteCommunicationSupport.Services;

public class KeyBuilderAndGeneratorService : IKeyBuilderAndGeneratorService
{
    private ConnectionMultiplexer redis;
    private ILogger<KeyBuilderAndGeneratorService> logger;

    public KeyBuilderAndGeneratorService(ConnectionMultiplexer redis, ILogger<KeyBuilderAndGeneratorService> logger)
    {
        this.redis = redis;
        this.logger = logger;
    }

    public async Task<int> NextInSequence(string sequenceKey)
    {
        IDatabase db = redis.GetDatabase();
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
