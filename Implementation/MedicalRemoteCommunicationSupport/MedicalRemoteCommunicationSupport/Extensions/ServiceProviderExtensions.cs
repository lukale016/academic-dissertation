using MedicalRemoteCommunicationSupport.Helpers;
using MedicalRemoteCommunicationSupport.Services;
using MongoDB.Driver;
using StackExchange.Redis;

namespace MedicalRemoteCommunicationSupport.Extensions;

public static class ServiceProviderExtensions
{
    public static IServiceCollection AddSingletons(this IServiceCollection services, IConfigurationSection dbSettings)
    {
        services.AddSingleton<UnitOfWork>();
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IFileManager, FileManager>();
        services.AddSingleton<ConnectionMultiplexer>(ConnectionMultiplexer.Connect(dbSettings["RedisConnectionUrl"]));
        services.AddSingleton<MongoClient>(new MongoClient(dbSettings["MongoConnectionUrl"]));
        services.AddSingleton<IConnectionManager, ConnectionManager>();
        services.AddSingleton<IFilterHelper, FilterHelper>();
        return services; 
    }
}
