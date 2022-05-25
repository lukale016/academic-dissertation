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
            return new JsonResult((await unitOfWork.PatientRepository.GetUser(username)));
        }
        catch (ResponseException ex)
        {
            if (ex.Status != StatusCodes.Status404NotFound && ex.Status != StatusCodes.Status500InternalServerError)
            {
                return StatusCode(ex.Status, ex.Message);
            }

            try
            {
                Doctor doctor =  (await unitOfWork.DoctorRepository.GetUser(username));
                return new JsonResult(doctor);
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

    [HttpPut]
    public async Task<ActionResult<Patient>> UpdatePatient([FromBody]Patient patient)
    {
        try
        {
            return await unitOfWork.PatientRepository.UpdatePatient(patient);
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }

    [HttpPut]
    public async Task<ActionResult<Doctor>> UpdateDoctor([FromBody]Doctor doctor)
    {
        try
        {
            return await unitOfWork.DoctorRepository.UpdateDoctor(doctor);
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }

    [HttpDelete("{username}")]
    public async Task<ActionResult> DeleteUser([FromRoute]string username)
    {
        try
        {
            // Doctors repository can also be used, since implementation does not differ
            await unitOfWork.PatientRepository.DeleteUser(username);
            return Ok("User is successfuly deleted");
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }
}
