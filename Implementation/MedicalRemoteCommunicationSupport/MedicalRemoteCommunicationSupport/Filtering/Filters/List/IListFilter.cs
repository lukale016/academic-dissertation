namespace MedicalRemoteCommunicationSupport.Filtering;

public interface IListFilter<TSearched, TCriteria> where TCriteria: IListCriteria<TSearched>
{
    IEnumerable<TSearched> Search(IEnumerable<TSearched> data, TCriteria criteria);
}