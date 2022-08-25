using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

namespace MedicalRemoteCommunicationSupport.Models;

[DataContract]
[BsonIgnoreExtraElements]
public class Appointment
{
    [BsonId]
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string DoctorRef { get; set; }

    [DataMember]
    public Doctor Doctor { get; set; }

    [DataMember]
    public string PatientRef { get; set; }

    [DataMember]
    public Patient Patient { get; set; }

    [DataMember]
    public DateTime ScheduledTime { get; set; }

    [DataMember]
    public int LengthInMins { get; set; }

    public Appointment(AppointmentPostDto dto)
    {
        DoctorRef = dto.Doctor;
        PatientRef = dto.Patient;
        ScheduledTime = dto.ScheduledTime;
        LengthInMins = dto.Duration;
    }
}
