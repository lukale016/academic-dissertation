using StackExchange.Redis;
using System.Text.Json;

namespace MedicalRemoteCommunicationSupport.Data.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly UnitOfWork unitOfWork;
    private readonly IConnectionMultiplexer redis;
    public CommentRepository(UnitOfWork unit, IConnectionMultiplexer redis)
    {
        unitOfWork = unit;
        this.redis = redis;
    }

    public async Task<Comment> AddCommentToPost(int id, Comment comment)
    {
        if(id <= 0 || comment is null  || string.IsNullOrWhiteSpace(comment.Owner))
        {
            throw new ResponseException(StatusCodes.Status400BadRequest, "Parameters not set");
        }
        
        UserBase user = null;
        if(comment.IsDoctorComment)
        {
            user = await unitOfWork.DoctorRepository.GetUser(comment.Owner);
        }
        else
        {
            user = await unitOfWork.PatientRepository.GetUser(comment.Owner);
        }
        if(user is null)
        {
            throw new ResponseException(StatusCodes.Status404NotFound, "User not found");
        }

        comment.UserFullName = user.FullName;

        Topic topic = await unitOfWork.TopicRepository.GetTopic(id);

        await redis.GetDatabase().ListLeftPushAsync(topic.CommentsKey, JsonSerializer.Serialize<Comment>(comment));
        return comment;
    }

    public async Task DeleteCommentFromPost(int id, Comment comment)
    {
        if (id <= 0 || comment is null || string.IsNullOrWhiteSpace(comment.Owner))
        {
            throw new ResponseException(StatusCodes.Status400BadRequest, "Parameters not set");
        }

        Topic topic = await unitOfWork.TopicRepository.GetTopic(id);

        IDatabase db = redis.GetDatabase();
        List<Comment> comments = (await db.ListRangeAsync(topic.CommentsKey))
                                    .Select(rv => JsonSerializer.Deserialize<Comment>(rv))
                                    .ToList();

        await db.ListRemoveAsync(topic.CommentsKey, JsonSerializer.Serialize<Comment>(comment));
    }
}
