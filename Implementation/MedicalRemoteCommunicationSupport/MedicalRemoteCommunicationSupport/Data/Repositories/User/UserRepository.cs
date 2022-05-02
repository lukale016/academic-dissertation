using MongoDB.Driver;

namespace MedicalRemoteCommunicationSupport.Data.Repositories;
public class UserRepository<T> : IUserRepository<T> where T : UserBase
{
    IMongoCollection<T> users;
    public UserRepository(IMongoDatabase mongoDb)
    {
        users = mongoDb.GetCollection<T>(CollectionConstants.Users);
    }

    public async Task AddUser(T user)
    {
        if(user == null || string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
        {
            throw new ResponseException(StatusCodes.Status400BadRequest, "Parameters not set");
        }

        var filter = Builders<T>.Filter.Eq(nameof(UserBase.Username), user.Username);
        UserBase existingUser = null;
        try
        {
            existingUser = (await users.FindAsync(filter)).SingleOrDefault();
        }
        catch(Exception ex)
        {
            throw new ResponseException(StatusCodes.Status500InternalServerError, ex.Message);
        }

        if(existingUser is not null)
        {
            throw new ResponseException(StatusCodes.Status409Conflict, "Already exists");
        }

        await users.InsertOneAsync(user);
    }

    public async Task DeleteUser(string username)
    {
        if(string.IsNullOrEmpty(username))
        {
            throw new ResponseException(StatusCodes.Status400BadRequest, "Parameters not set");
        }
        var filter = Builders<T>.Filter.Eq(nameof(UserBase.Username), username);
        UserBase existingUser = null;
        try
        {
            existingUser = (await users.FindAsync(filter)).SingleOrDefault();
        }
        catch(Exception ex)
        {
            throw new ResponseException(StatusCodes.Status500InternalServerError, ex.Message);
        }
        if(existingUser is null)
        {
            throw new ResponseException(StatusCodes.Status404NotFound, "User does not exist");
        }
        await users.DeleteOneAsync(filter);
    }

    public async Task<T> GetUser(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ResponseException(StatusCodes.Status400BadRequest, "Parameters not set");
        }

        var filter = Builders<T>.Filter.Eq(nameof(UserBase.Username), username);
        UserBase user = null;
        try
        {
            user = (await users.FindAsync(filter)).SingleOrDefault();
        }
        catch(Exception ex)
        {
            throw new ResponseException(StatusCodes.Status500InternalServerError, ex.Message);
        }
        if (user is not T)
        {
            throw new ResponseException(StatusCodes.Status404NotFound, "User not found");
        }
        return user as T;
    }
}
