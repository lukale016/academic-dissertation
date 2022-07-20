namespace MedicalRemoteCommunicationSupport.Data.Repositories;

public interface IMessageRepository
{
    Task<Message> StoreMessage(string receiver, Message message);
    Task<IEnumerable<Message>> GetMessages(string sender, string receiver);
}
