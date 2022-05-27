using MongoDB.Driver;

namespace MedicalRemoteCommunicationSupport.Filtering;

public class MongoFilter<T, Q> : IMongoFilter<T, Q> where Q : ICriteria<T>
{
    private IMongoCollection<T> collection;
    public MongoFilter(IMongoDatabase mongo)
    {
        string collectionName = null;
        Type type = typeof(T);
        if(type == typeof(Patient) || type == typeof(Doctor))
        {
            collectionName = CollectionConstants.Users;
        }
        else if (type == typeof(Topic))
        {
            collectionName = CollectionConstants.Topics;
        }
        else if (type == typeof(Appointment))
        {
            collectionName = CollectionConstants.Appointments;
        }

        if (collectionName is null)
        {
            throw new ResponseException(StatusCodes.Status500InternalServerError, "Unsuported mongo model");
        }

        collection = mongo.GetCollection<T>(collectionName);
    }

    public async Task<IEnumerable<T>> Search(Q criteria)
    {
        return (await collection.FindAsync(criteria.BuildFilter())).ToList();
    }
}
