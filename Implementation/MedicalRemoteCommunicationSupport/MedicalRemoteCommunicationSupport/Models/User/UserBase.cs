﻿using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

namespace MedicalRemoteCommunicationSupport.Models;

[DataContract]
[KnownType(typeof(Doctor))]
[KnownType(typeof(Patient))]
public abstract class UserBase
{
    [BsonId]
    [DataMember]
    public string Username { get; set; }

    [DataMember]
    public string Password { get; set; }

    [DataMember]
    public string SkypeId { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string MiddleName { get; set; }

    [DataMember]
    public string Surname { get; set; }

    [BsonIgnore]
    public virtual string FullName => $"{Name} {MiddleName} {Surname}";

    [DataMember]
    public string Gender { get; set; }

    [DataMember]
    public DateTime DateOfBirth { get; set; }

    [DataMember]
    public bool IsDoctor { get; protected set; }

    [BsonIgnore]
    public string AppointmentDatesListKey => $"AppointmentDates:{Username}";
    
    [BsonIgnore]
    public List<Appointment> Appointments { get; set; } = new();

    public string MessageKeyForUser(string receivedFrom)
    {
        return $"{receivedFrom}:SentMessagesTo:{Username}";
    }
}
