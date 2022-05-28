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

    public Patient(PatientPostDto dto)
    {
        Username = dto.Username;
        Password = dto.Password;
        Name = dto.Name;
        MiddleName = dto.MiddleName;
        Surname = dto.Surname;
        Gender = dto.Gender;
        IsDoctor = false;
    }
}
