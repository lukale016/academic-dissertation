namespace MedicalRemoteCommunicationSupport.Filtering;

public interface IMongoFilter<T, Q> where Q : ICriteria<T>
{
    Task<IEnumerable<T>> Search(Q criteria);
}
