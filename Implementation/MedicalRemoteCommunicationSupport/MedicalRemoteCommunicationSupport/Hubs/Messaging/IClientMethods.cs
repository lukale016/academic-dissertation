namespace MedicalRemoteCommunicationSupport.Hubs.Messaging;

public interface IClientMethods
{
    Task ReceiveMessage(Message message);
}
