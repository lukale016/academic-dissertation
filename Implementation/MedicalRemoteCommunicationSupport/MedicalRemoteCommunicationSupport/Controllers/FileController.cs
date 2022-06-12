using MedicalRemoteCommunicationSupport.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedicalRemoteCommunicationSupport.Controllers;

[Route("api/[controller]/[action]")]
public class FileController : Controller
{
    private readonly IFileManager fileManager;

    public FileController(IFileManager manager)
    {
        this.fileManager = manager;
    }

    [HttpGet("{fileName}")]
    public async Task<ActionResult> GetFile([FromRoute]string fileName)
    {
        try
        {
            HttpFileData file = await fileManager.GetFile(fileName);
            return File(file.Data, file.ContentType, file.Name);
        }
        catch(ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult> StoreFile([FromForm]IFormFile file)
    {
        try
        {
            await fileManager.SaveFile(file);
            return Ok("File stored");
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }
}
