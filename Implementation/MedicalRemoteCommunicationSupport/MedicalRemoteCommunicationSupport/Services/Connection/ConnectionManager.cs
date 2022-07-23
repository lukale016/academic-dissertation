using Ardalis.GuardClauses;
using StackExchange.Redis;

namespace MedicalRemoteCommunicationSupport.Services;

public class ConnectionManager : IConnectionManager
{
    private Dictionary<string, string> cache;
    private readonly ConnectionMultiplexer redis;
    private TimeSpan expireTime = TimeSpan.FromMinutes(30);

    public ConnectionManager(ConnectionMultiplexer redis)
    {
        this.redis = redis;
        this.cache = new();
    }

    public void RegisterConnection(string username, string connectionId)
    {
        Guard.Against.NullOrEmpty(username, nameof(username));
        Guard.Against.NullOrEmpty(connectionId, nameof(connectionId));
        if(cache.ContainsKey(username))
            cache[username] = connectionId;
        else
            cache.Add(username, connectionId);
        redis.GetDatabase().StringSetAsync(username, connectionId);
        Task.Delay(expireTime).ContinueWith(_ => CacheExpired(username));
    }

    public void RemoveConnection(string username)
    {
        Guard.Against.NullOrEmpty(username, nameof(username));
        cache.Remove(username);
        redis.GetDatabase().KeyDeleteAsync(username);
    }

    public async Task<string> GetConnectionId(string username)
    {
        Guard.Against.NullOrEmpty(username, nameof(username));
        string result = string.Empty;
        if (cache.TryGetValue(username, out result))
            return result;
        result =  await redis.GetDatabase().StringGetAsync(username);
        return result == null ? string.Empty : result;
    }

    private Task CacheExpired(string key)
    {
        cache.Remove(key);
        return Task.CompletedTask;
    }
}
