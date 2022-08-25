namespace MedicalRemoteCommunicationSupport.Records;

public record struct Credentials(string Username, string Password);
public record UserAndToken(object User, string Token);
public record DoctorPostDto(string Username, string Password, string Email, string Name, string MiddleName, string Surname, string Gender, DateTime DateOfBirth, string Specialization);
public record PatientPostDto(string Username, string Password, string Email, string Name, string MiddleName, string Surname, string Gender, DateTime DateOfBirth);
public record AppointmentPostDto(string Doctor, string Patient, DateTime ScheduledTime, int Duration);
public record struct DoctorRequestDto(string Username, string FullName);
public record struct PatientRequestDto(string Username, string Specialization);
public record struct MyConnection(string Username, string FullName);
public record struct RequestRejectionData(string RejectedUsername, string DoctorFullName);
public record struct FileGuid(string guid);