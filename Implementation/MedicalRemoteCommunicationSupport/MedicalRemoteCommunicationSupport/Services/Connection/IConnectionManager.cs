namespace MedicalRemoteCommunicationSupport.Services;

public interface IConnectionManager
{
    void RegisterConnection(string username, string connectionId);
    void RemoveConnection(string username);
    Task<string> GetConnectionId(string username);
}
