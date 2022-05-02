namespace MedicalRemoteCommunicationSupport.Data.Repositories;
public interface IUserRepository<T> where T : UserBase
{
    Task<T> GetUser(string username);
    Task AddUser(T user);
    Task DeleteUser(string username);
}
