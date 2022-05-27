namespace MedicalRemoteCommunicationSupport.Services;

public interface IFileManager
{
    Task<HttpFileData> GetFile(string fileName);
    Task SaveFile(IFormFile file);
    void DeleteFile(string fileName);
}
