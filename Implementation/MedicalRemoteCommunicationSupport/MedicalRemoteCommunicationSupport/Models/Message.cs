namespace MedicalRemoteCommunicationSupport.Models;

public class Message
{
    public string From { get; set; }
    public string To { get; set; }
    public string Content { get; set; }
    /// <summary>
    /// Used for api route for file download
    /// </summary>
    public string Link { get; set; }
}
