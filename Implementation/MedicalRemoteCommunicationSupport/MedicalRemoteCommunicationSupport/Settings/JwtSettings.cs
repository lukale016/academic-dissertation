namespace MedicalRemoteCommunicationSupport.Settings;

public class JwtSettings
{
    public string SigningKey { get; set; }
    public string Issuer { get; set; }
    public string Audiance { get; set; }
}
