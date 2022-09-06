namespace MedicalRemoteCommunicationSupport.Hubs.Messaging;

public interface IClientMethods
{
    Task ReceiveMessage(string from, string content, DateTime time);
    Task RequestSent(string username, string specialization);
    Task RequestReceived(string username, string fullName);
    Task RequestAccepted(string username, string fullName, string skypeId);
    Task RequestFinished(string username, string fullName, string skypeId);
    Task RequestRejected(string username);
    Task RequestHasBeenRejected(string username, string fullName);
}
