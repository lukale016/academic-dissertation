using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

namespace MedicalRemoteCommunicationSupport.Models;

[DataContract]
public class Doctor : UserBase
{
    [BsonIgnore]
    public override string FullName => $"Dr. {base.FullName}";

    [BsonIgnore]
    public string RequestListKey => $"Requests:{Username}";

    [BsonIgnore]
    public IEnumerable<RequestDto> Requests { get; set; }

    [BsonIgnore]
    public string PatientListKey => $"Patients:{Username}";

    [BsonIgnore]
    public IEnumerable<MyConnection> Patients { get; set; } = Enumerable.Empty<MyConnection>();

    [DataMember]
    public string Specialization { get; set; }

    [DataMember]
    public string StartTime { get; set; }

    [DataMember]
    public string EndTime { get; set; }

    public Doctor(DoctorPostDto dto)
    {
        Username = dto.Username;
        Password = dto.Password;
        Email = dto.Email;
        Name = dto.Name;
        MiddleName = dto.MiddleName;
        Surname = dto.Surname;
        Gender = dto.Gender;
        DateOfBirth = dto.DateOfBirth;
        Specialization = dto.Specialization;
        StartTime = TimeConstants.StartTime;
        EndTime = TimeConstants.EndTime;
        IsDoctor = true;
    }
}
