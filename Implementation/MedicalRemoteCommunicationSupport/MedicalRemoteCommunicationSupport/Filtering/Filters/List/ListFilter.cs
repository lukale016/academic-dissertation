namespace MedicalRemoteCommunicationSupport.Filtering;

public class ListFilter<TSearched, TCriteria> : IListFilter<TSearched, TCriteria> where TCriteria : IListCriteria<TSearched>
{
    public IEnumerable<TSearched> Search(IEnumerable<TSearched> data, TCriteria criteria)
    {
        return criteria.Filter(data);
    }
}
