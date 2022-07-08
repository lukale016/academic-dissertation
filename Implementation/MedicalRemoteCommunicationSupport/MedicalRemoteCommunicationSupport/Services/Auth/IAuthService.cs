namespace MedicalRemoteCommunicationSupport.Services;

public interface IAuthService
{
    Task<UserAndToken> Login(Credentials creds);
}
