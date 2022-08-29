using MedicalRemoteCommunicationSupport.Filtering;

namespace MedicalRemoteCommunicationSupport.Helpers;

public interface IFilterHelper
{
    IMongoFilter<T, Q> ReturnMongoFiltrator<T, Q>() where Q : IMongoCriteria<T>;

    IListFilter<TSearched, TCriteria> ReturnListFiltrator<TSearched, TCriteria>() where TCriteria : IListCriteria<TSearched>;
}
