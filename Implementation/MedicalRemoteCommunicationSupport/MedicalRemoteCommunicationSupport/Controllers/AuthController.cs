using MedicalRemoteCommunicationSupport.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalRemoteCommunicationSupport.Controllers;

[Route("api/[controller]/[action]")]
public class AuthController : Controller
{
    private readonly IAuthService auth;
    private readonly UnitOfWork unitOfWork;

    public AuthController(IAuthService authService, UnitOfWork unit)
    {
        auth = authService;
        unitOfWork = unit;
    }

    [HttpPost]
    public async Task<ActionResult<UserAndToken>> Login([FromBody]Credentials creds)
    {
        try
        {
            return await auth.Login(creds);
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<object> GetUserFromToken()
    {
        var claim = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
        if (claim is null)
            return StatusCode(StatusCodes.Status401Unauthorized, "Token not provided");
        try
        {
            Patient patient = (await unitOfWork.PatientRepository.GetUser(claim.Value));
            return new JsonResult(patient);
        }
        catch (ResponseException ex)
        {
            if (ex.Status != StatusCodes.Status404NotFound && ex.Status != StatusCodes.Status500InternalServerError)
            {
                return StatusCode(ex.Status, ex.Message);
            }

            try
            {
                Doctor doctor = (await unitOfWork.DoctorRepository.GetUser(claim.Value));
                return new JsonResult(doctor);
            }
            catch (ResponseException reThrow)
            {
                return StatusCode(reThrow.Status, reThrow.Message);
            }
        }
        catch(Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
