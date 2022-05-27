using MongoDB.Driver;

namespace MedicalRemoteCommunicationSupport.Filtering;
public interface ICriteria<T>
{
    FilterDefinition<T> BuildFilter();
}
