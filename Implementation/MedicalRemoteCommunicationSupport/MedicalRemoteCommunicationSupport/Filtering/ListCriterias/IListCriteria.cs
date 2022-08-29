namespace MedicalRemoteCommunicationSupport.Filtering;

public interface IListCriteria<TFiltered>
{
    IEnumerable<TFiltered> Filter(IEnumerable<TFiltered> data);
}
