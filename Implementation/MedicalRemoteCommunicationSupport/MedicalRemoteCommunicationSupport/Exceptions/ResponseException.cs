namespace MedicalRemoteCommunicationSupport.Exceptions;
public class ResponseException: Exception
{
    public int Status { get; set; }

    public ResponseException(int status, string message): base(message)
    {
        Status = status;
    }
}
