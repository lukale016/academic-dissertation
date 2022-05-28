using Microsoft.AspNetCore.Mvc;

namespace MedicalRemoteCommunicationSupport.Controllers;

[Route("[controller]/[action]")]
public class AppointmentController : Controller
{
    private readonly UnitOfWork unitOfWork;

    public AppointmentController(UnitOfWork unit)
    {
        this.unitOfWork = unit;
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<Dictionary<string, IEnumerable<Appointment>>>> GetAppointments([FromRoute]string username)
    {
        try
        {
            return new JsonResult(await unitOfWork.AppointmentRepository.GetAppointments(username));
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
}
