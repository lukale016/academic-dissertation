namespace MedicalRemoteCommunicationSupport.Data.Repositories;

public interface ITopicRepostiory
{
    Task<IEnumerable<Topic>>GetAllTopics();
    Task<Topic> GetTopic(int id);
    Task<Topic> AddTopic(Topic topic);
    Task<Topic> UpdateTopic(Topic topic);
    Task<int> DeleteTopic(int id);
}
