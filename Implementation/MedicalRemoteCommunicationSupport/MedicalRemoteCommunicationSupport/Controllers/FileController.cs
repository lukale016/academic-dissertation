using MedicalRemoteCommunicationSupport.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalRemoteCommunicationSupport.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
public class FileController : Controller
{
    private readonly IFileManager fileManager;

    public FileController(IFileManager manager)
    {
        this.fileManager = manager;
    }

    [AllowAnonymous]
    [HttpGet("{fileGuid}")]
    public async Task<ActionResult> GetFile([FromRoute]string fileGuid)
    {
        try
        {
            HttpFileData file = await fileManager.GetFile(fileGuid);
            return File(file.Data, file.ContentType, file.Name);
        }
        catch(ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<FileGuid>> StoreFile([FromForm]IFormFile file)
    {
        try
        {
            return await fileManager.SaveFile(file);
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }
}
