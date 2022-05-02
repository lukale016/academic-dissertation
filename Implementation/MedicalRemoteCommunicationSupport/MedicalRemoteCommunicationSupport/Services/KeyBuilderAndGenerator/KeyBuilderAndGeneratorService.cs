using StackExchange.Redis;

namespace MedicalRemoteCommunicationSupport.Services.KeyBuilderAndGenerator;

public class KeyBuilderAndGeneratorService : IKeyBuilderAndGeneratorService
{
    private string dateFormat = "dd-MM-yyyy";
    private DateTime referenceDate = new DateTime(2022, 1, 1);
    private ConnectionMultiplexer redis;
    private ILogger<KeyBuilderAndGeneratorService> logger;

    public KeyBuilderAndGeneratorService(ConnectionMultiplexer redis, ILogger<KeyBuilderAndGeneratorService> logger)
    {
        this.redis = redis;
        this.logger = logger;
    }

    public string BuildRedisKey(string doctor, DateTime date)
    {
        return $"{doctor}_{date.ToString(dateFormat)}";
    }

    public async Task<int> NextInSequance(string sequanceKey)
    {
        IDatabase db = redis.GetDatabase();
        if(db.KeyExists(sequanceKey))
        {
            int id;
            try
            {
                (await db.StringGetAsync(sequanceKey)).TryParse(out id);
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                throw new ResponseException(500, ex.Message);
            }
            await db.StringSetAsync(sequanceKey, id + 1);
            return id;
        }
        await db.StringSetAsync(sequanceKey, 1);
        return 1;
    }

    public long ScoreFromTime(DateTime time)
    {
        return referenceDate.Ticks - time.Ticks;
    }
}
