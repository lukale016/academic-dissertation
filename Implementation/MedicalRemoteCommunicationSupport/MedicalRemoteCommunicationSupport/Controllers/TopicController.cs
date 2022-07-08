using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalRemoteCommunicationSupport.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
public class TopicController : Controller
{
    private readonly UnitOfWork unitOfWork;

    public TopicController(UnitOfWork unit)
    {
        unitOfWork = unit;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Topic>>> GetTopics()
    {
        try
        {
            return new JsonResult(await unitOfWork.TopicRepostiory.GetAllTopics());
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<Topic>> GetTopic([FromRoute]int id)
    {
        try
        {
            return await unitOfWork.TopicRepostiory.GetTopic(id);
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<Topic>> AddTopic([FromBody]Topic topic)
    {
        try
        {
            return await unitOfWork.TopicRepostiory.AddTopic(topic);
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }

    [HttpPut]
    public async Task<ActionResult<Topic>> UpdateTopic([FromBody]Topic topic)
    {
        try
        {
            return await unitOfWork.TopicRepostiory.UpdateTopic(topic);
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<int>> DeleteTopic([FromRoute]int id)
    {
        try
        {
            return await unitOfWork.TopicRepostiory.DeleteTopic(id);
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }
}
