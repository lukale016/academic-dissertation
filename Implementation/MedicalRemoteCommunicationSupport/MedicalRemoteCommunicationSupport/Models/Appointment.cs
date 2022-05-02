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

    [BsonIgnore]
    public Doctor Doctor { get; set; }

    [DataMember]
    public string PatientRef { get; set; }

    [BsonIgnore]
    public Patient Patient { get; set; }

    [DataMember]
    public DateTime ScheduledTimeStart { get; set; }

    [DataMember]
    public DateTime ScheduledTimeEnd { get; set; }
}
