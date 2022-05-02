using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

namespace MedicalRemoteCommunicationSupport.Models;

[DataContract]
public class Doctor : UserBase
{
    [BsonIgnore]
    public override bool IsDoctor => true;
    
    [BsonIgnore]
    public override string FullName => $"Dr. {base.FullName}";
    
    [DataMember]
    public string Specialization { get; set; }

    [DataMember]
    public string StartTime { get; set; }

    [DataMember]
    public string EndTime { get; set; }

    [BsonIgnore]
    public List<Appointment> Appointments { get; set; }

    public Doctor(DoctorPostDto dto)
    {
        Username = dto.Username;
        Password = dto.Password;
        Name = dto.Name;
        MiddleName = dto.MiddleName;
        Surname = dto.Surname;
        Gender = dto.Gender;
        Specialization = dto.Specialization;
        StartTime = TimeConstants.StartTime;
        EndTime = TimeConstants.EndTime;
    }
}
