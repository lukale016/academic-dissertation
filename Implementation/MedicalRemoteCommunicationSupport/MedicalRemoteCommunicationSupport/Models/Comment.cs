using System.Text.Json.Serialization;

namespace MedicalRemoteCommunicationSupport.Models;
public class Comment
{
    public string Owner { get; set; }
    public string UserFullName { get; set; }
    public string Description { get; set; }
    public bool IsDoctorComment { get; set; }
}
