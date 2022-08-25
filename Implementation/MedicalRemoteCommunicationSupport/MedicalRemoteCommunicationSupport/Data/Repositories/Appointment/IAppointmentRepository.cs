using MedicalRemoteCommunicationSupport.Filtering;

namespace MedicalRemoteCommunicationSupport.Data.Repositories;

public interface IAppointmentRepository
{
    Task<Dictionary<string, IEnumerable<Appointment>>> GetAppointments(string username);
    Task<Dictionary<string, IEnumerable<Appointment>>> Search(string username, AppointmentListCriteria criteria);
    Task<IEnumerable<string>> OccupiedTimeSlots(string username, string scheduledDate);
    Task<Appointment> RegisterAppointment(AppointmentPostDto dto);
    Task<int> DeleteAppointment(int id);
}
