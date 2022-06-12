namespace MedicalRemoteCommunicationSupport.Records;

public record struct Credentials(string Username, string Password);
public record DoctorPostDto(string Username, string Password, string Email, string Name, string MiddleName, string Surname, string Gender, DateTime DateOfBirth, string Specialization);
public record PatientPostDto(string Username, string Password, string Email, string Name, string MiddleName, string Surname, string Gender, DateTime DateOfBirth);
public record AppointmentPostDto(string Doctor, string Patient, DateTime Date, DateTime StartTime, DateTime EndTime);
public record struct RequestDto(string Username, string Name, string MiddleName, string Surname);