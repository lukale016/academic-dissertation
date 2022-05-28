namespace MedicalRemoteCommunicationSupport.Records;

public record DoctorPostDto(string Username, string Password, string Name, string MiddleName, string Surname, string Gender, string Specialization);
public record PatientPostDto(string Username, string Password, string Name, string MiddleName, string Surname, string Gender);
public record AppointmentPostDto(string Doctor, string Patient, DateTime Date, DateTime StartTime, DateTime EndTime);
public record struct RequestDto(string Username, string Name, string MiddleName, string Surname);