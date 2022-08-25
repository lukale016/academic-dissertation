namespace MedicalRemoteCommunicationSupport.Services;

public interface IConnectionManager
{
    Task RegisterConnection(string username, string connectionId);
    Task RemoveConnection(string username);
    Task<string> GetConnectionId(string username);
}
