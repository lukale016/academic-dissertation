using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

namespace MedicalRemoteCommunicationSupport.Models;

[DataContract]
[BsonIgnoreExtraElements]
public class Patient : UserBase
{
    [BsonIgnore]
    public List<Appointment> Appointments { get; set; }

    public Patient(PatientPostDto dto)
    {
        Username = dto.Username;
        Password = dto.Password;
        Name = dto.Name;
        MiddleName = dto.MiddleName;
        Surname = dto.Surname;
        Gender = dto.Gender;
    }
}
