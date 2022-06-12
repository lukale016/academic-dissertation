namespace MedicalRemoteCommunicationSupport.Services;

public interface IAuthService
{
    Task<object> LogIn(Credentials creds);
}
