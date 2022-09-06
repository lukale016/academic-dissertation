using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

namespace MedicalRemoteCommunicationSupport.Models;

[DataContract]
public class Patient : UserBase
{
    [BsonIgnore]
    public string CreatedTopicsKey => $"CreatedTopics:{Username}";

    [BsonIgnore]
    public List<Topic> CreatedTopics { get; set; }

    [BsonIgnore]
    public string SentRequestsListKey => $"SentRequests:{Username}";

    [BsonIgnore]
    public IEnumerable<PatientRequestDto> SentRequests { get; set; }

    [BsonIgnore]
    public string MyDoctorsListKey => $"Doctors:{Username}";

    [BsonIgnore]
    public IEnumerable<MyConnection> MyDoctors { get; set; } = Enumerable.Empty<MyConnection>();

    public Patient(PatientPostDto dto)
    {
        Username = dto.Username;
        Password = dto.Password;
        SkypeId = dto.SkypeId;
        Name = dto.Name;
        MiddleName = dto.MiddleName;
        Surname = dto.Surname;
        Gender = dto.Gender;
        DateOfBirth = dto.DateOfBirth;
        IsDoctor = false;
    }
}
