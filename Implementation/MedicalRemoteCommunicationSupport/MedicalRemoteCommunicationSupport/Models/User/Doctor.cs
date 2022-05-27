﻿using MongoDB.Bson.Serialization.Attributes;
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
    public IEnumerable<string> Requests { get; set; } = new();

    [BsonIgnore]
    public string PatientListKey => $"Patients:{Username}";

    [BsonIgnore]
    public IEnumerable<Patient> Patients { get; set; } = new();

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
        Name = dto.Name;
        MiddleName = dto.MiddleName;
        Surname = dto.Surname;
        Gender = dto.Gender;
        Specialization = dto.Specialization;
        StartTime = TimeConstants.StartTime;
        EndTime = TimeConstants.EndTime;
        IsDoctor = true;
    }
}