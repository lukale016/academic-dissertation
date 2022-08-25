using MedicalRemoteCommunicationSupport.Filtering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalRemoteCommunicationSupport.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
public class AppointmentController : Controller
{
    private readonly UnitOfWork unitOfWork;

    public AppointmentController(UnitOfWork unit)
    {
        this.unitOfWork = unit;
    }

    [HttpGet]
    public async Task<ActionResult<Dictionary<string, IEnumerable<Appointment>>>> GetAppointments()
    {
        string username = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).SingleOrDefault().Value;
        try
        {
            return new JsonResult(await unitOfWork.AppointmentRepository.GetAppointments(username));
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<string>>> OccupiedTimeSlots(string scheduledTime)
    {
        string username = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).SingleOrDefault().Value;
        try
        {
            return new JsonResult((await unitOfWork.AppointmentRepository.OccupiedTimeSlots(username, scheduledTime)));
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<Appointment>> RegisterAppointment([FromBody]AppointmentPostDto dto)
    {
        try
        {
            return await unitOfWork.AppointmentRepository.RegisterAppointment(dto);
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<int>> DeleteAppointment([FromRoute]int id)
    {
        try
        {
            return await unitOfWork.AppointmentRepository.DeleteAppointment(id);
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<Dictionary<string, IEnumerable<Appointment>>>> Search([FromBody]AppointmentListCriteria criteria)
    {
        string username = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).SingleOrDefault().Value;
        try
        {
            return await unitOfWork.AppointmentRepository.Search(username, criteria);
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }
}
