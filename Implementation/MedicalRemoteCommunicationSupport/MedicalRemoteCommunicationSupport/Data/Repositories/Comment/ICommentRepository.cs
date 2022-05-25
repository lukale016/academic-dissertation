namespace MedicalRemoteCommunicationSupport.Data.Repositories;

public interface ICommentRepository
{
    Task<Comment> AddCommentToPost(int id, Comment comment);
    Task DeleteCommentFromPost(int id, Comment comment);
}
