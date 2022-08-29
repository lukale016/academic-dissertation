using MedicalRemoteCommunicationSupport.Filtering;
using MedicalRemoteCommunicationSupport.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MedicalRemoteCommunicationSupport.Helpers;

public class FilterHelper : IFilterHelper
{
    private readonly IMongoDatabase mongoDb;
    public FilterHelper(MongoClient mongo, IOptions<DbSettings> dbConfig)
    {
        mongoDb = mongo.GetDatabase(dbConfig.Value.MongoDefaultDb);
    }

    public IListFilter<TSearched, TCriteria> ReturnListFiltrator<TSearched, TCriteria>() where TCriteria : IListCriteria<TSearched>
    {
        return new ListFilter<TSearched, TCriteria>();
    }

    public IMongoFilter<T, Q> ReturnMongoFiltrator<T, Q>() where Q : IMongoCriteria<T>
    {
        return new MongoFilter<T, Q>(mongoDb);
    }
}
