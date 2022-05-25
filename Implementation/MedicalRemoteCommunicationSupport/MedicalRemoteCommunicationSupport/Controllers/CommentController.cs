using Microsoft.AspNetCore.Mvc;

namespace MedicalRemoteCommunicationSupport.Controllers;

[Route("[controller]/[action]")]
public class CommentController : Controller
{
    private readonly UnitOfWork unitOfWork;

    public CommentController(UnitOfWork unit)
    {
        unitOfWork = unit;
    }

    [HttpPost("{id}")]
    public async Task<ActionResult<Comment>> AddCommentToPost([FromRoute]int id, [FromBody]Comment comment)
    {
        try
        {
            return await unitOfWork.CommentRepository.AddCommentToPost(id, comment);
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCommentFromPost([FromRoute]int id, [FromBody]Comment comment)
    {
        try
        {
            await unitOfWork.CommentRepository.DeleteCommentFromPost(id, comment);
            return Ok("Commennt deleted");
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }
}
