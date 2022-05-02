using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

namespace MedicalRemoteCommunicationSupport.Models;

[DataContract]
[KnownType(typeof(Doctor))]
[KnownType(typeof(Patient))]
[BsonIgnoreExtraElements]
public abstract class UserBase
{
    [BsonId]
    [DataMember]
    public string Username { get; set; }

    [DataMember]
    public string Password { get; set; }

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

    [BsonIgnore]
    public virtual bool IsDoctor => false;

}
