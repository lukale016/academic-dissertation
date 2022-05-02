using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

namespace MedicalRemoteCommunicationSupport.Models;

[DataContract]
[BsonIgnoreExtraElements]
public class Topic
{
    [BsonId]
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Title { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public string Owner { get; set; }

    [BsonIgnore]
    public string CommentsKey => $"Topic({Id})_{Title}_CommentList";

    [BsonIgnore]
    public List<Comment> Comments { get; set; }
}
