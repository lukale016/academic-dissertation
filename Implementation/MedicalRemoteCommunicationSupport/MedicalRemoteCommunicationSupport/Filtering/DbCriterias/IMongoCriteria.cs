using MongoDB.Driver;

namespace MedicalRemoteCommunicationSupport.Filtering;

public interface IMongoCriteria<T>
{
    FilterDefinition<T> BuildFilter();
}
