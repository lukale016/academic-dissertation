namespace MedicalRemoteCommunicationSupport.Filtering;

public interface IMongoFilter<T, Q> where Q : IMongoCriteria<T>
{
    Task<IEnumerable<T>> Search(Q criteria);
}
