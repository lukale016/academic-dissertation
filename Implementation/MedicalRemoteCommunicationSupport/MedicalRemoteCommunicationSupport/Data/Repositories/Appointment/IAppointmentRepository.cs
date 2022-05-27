namespace MedicalRemoteCommunicationSupport.Data.Repositories;

public interface IAppointmentRepository
{
    Task<Dictionary<string, IEnumerable<Appointment>>> GetAppointments(string username);
    Task<Appointment> RegisterAppointment(AppointmentPostDto dto);
    Task<int> DeleteAppointment(int id);
}
