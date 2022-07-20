namespace MedicalRemoteCommunicationSupport.Hubs.Messaging;

public interface IClientMethods
{
    Task ReceiveMessage(string from, string content, DateTime time);
}
