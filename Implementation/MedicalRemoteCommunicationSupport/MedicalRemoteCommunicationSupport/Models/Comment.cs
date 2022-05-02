namespace MedicalRemoteCommunicationSupport.Models;
public class Comment
{
    public string Owner { get; set; }
    public string MyProperty { get; set; }
    public int IsDeleted { get; set; }
    public bool IsDoctorComment { get; set; }
}
