using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalRemoteCommunicationSupport.Controllers;

[Authorize]
[Route("[controller]/[action]")]
public class MessageController : Controller
{
    private readonly UnitOfWork unitOfWork;

    public MessageController(UnitOfWork unit)
    {
        this.unitOfWork = unit;
    }

    [HttpGet("{receiver}")]
    public async Task<ActionResult<IEnumerable<Message>>> MessagesForUser([FromRoute]string receiver)
    {
        Claim claim = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
        if (claim is null)
            return Unauthorized();
        try
        {
            return new JsonResult(await unitOfWork.MessageRepository.GetMessages(claim.Value, receiver));
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }
}
