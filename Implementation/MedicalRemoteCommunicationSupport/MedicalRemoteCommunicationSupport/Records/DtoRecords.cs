namespace MedicalRemoteCommunicationSupport.Records;

public record DoctorPostDto(string Username, string Password, string Name, string MiddleName, string Surname, string Gender, string Specialization);
public record PatientPostDto(string Username, string Password, string Name, string MiddleName, string Surname, string Gender);