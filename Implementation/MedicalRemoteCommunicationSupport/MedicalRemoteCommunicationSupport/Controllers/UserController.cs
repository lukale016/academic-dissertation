using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MedicalRemoteCommunicationSupport.Controllers;

[Route("[controller]/[action]")]
public class UserController : Controller
{
    private readonly UnitOfWork unitOfWork;

    public UserController(UnitOfWork unit)
    {
        unitOfWork = unit;
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<object>> GetUser([FromRoute]string username)
    {
        try
        {
            return (await unitOfWork.PatientRepository.GetUser(username)) as Patient;
        }
        catch (ResponseException ex)
        {
            if (ex.Status != StatusCodes.Status404NotFound)
            {
                return StatusCode(ex.Status, ex.Message);
            }

            try
            {
                Doctor doctor =  (await unitOfWork.DoctorRepository.GetUser(username));
                return doctor;
            }
            catch(ResponseException reThrow)
            {
                return StatusCode(reThrow.Status, reThrow.Message);
            }
        }
    }

    [HttpPost]
    public async Task<ActionResult> AddDoctor([FromBody]DoctorPostDto dto)
    {
        try
        {
            await unitOfWork.DoctorRepository.AddUser(new Doctor(dto));
            return Ok("Doctor added");
        }
        catch (ResponseException reThrow)
        {
            return StatusCode(reThrow.Status, reThrow.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult> AddPatient([FromBody]PatientPostDto dto)
    {
        try
        {
            await unitOfWork.PatientRepository.AddUser(new Patient(dto));
            return Ok("Patient added");
        }
        catch (ResponseException reThrow)
        {
            return StatusCode(reThrow.Status, reThrow.Message);
        }
    }
}
