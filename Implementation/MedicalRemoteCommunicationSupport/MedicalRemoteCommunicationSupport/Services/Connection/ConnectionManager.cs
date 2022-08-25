using Ardalis.GuardClauses;
using StackExchange.Redis;

namespace MedicalRemoteCommunicationSupport.Services;

public class ConnectionManager : IConnectionManager
{
    private readonly ConnectionMultiplexer redis;
    private static TimeSpan expireTime = TimeSpan.FromHours(2);

    public ConnectionManager(ConnectionMultiplexer redis)
    {
        this.redis = redis;
    }

    public async Task RegisterConnection(string username, string connectionId)
    {
        Guard.Against.NullOrEmpty(username, nameof(username));
        Guard.Against.NullOrEmpty(connectionId, nameof(connectionId));
        IDatabase db = redis.GetDatabase();
        await db.StringSetAsync(username, connectionId);
        await db.KeyExpireAsync(username, expireTime);
    }

    public async Task RemoveConnection(string username)
    {
        Guard.Against.NullOrEmpty(username, nameof(username));
        await redis.GetDatabase().KeyDeleteAsync(username);
    }

    public async Task<string> GetConnectionId(string username)
    {
        Guard.Against.NullOrEmpty(username, nameof(username));
        string result =  await redis.GetDatabase().StringGetAsync(username);
        return result == null ? string.Empty : result;
    }
}
