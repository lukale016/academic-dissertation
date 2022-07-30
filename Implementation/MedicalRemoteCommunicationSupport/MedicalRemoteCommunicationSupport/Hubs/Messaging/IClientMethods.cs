namespace MedicalRemoteCommunicationSupport.Hubs.Messaging;

public interface IClientMethods
{
    Task ReceiveMessage(string from, string content, DateTime time);
    Task RequestReceived(string username, string fullName);
    Task RequestAccepted(string username, string fullName);
}
