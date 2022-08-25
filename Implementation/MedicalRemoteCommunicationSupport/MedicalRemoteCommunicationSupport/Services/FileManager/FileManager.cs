namespace MedicalRemoteCommunicationSupport.Services;

public class FileManager : IFileManager
{
    private List<(string fileName, string fileGuid, string contentType)> storedFiles = new();

    public void DeleteFile(string fileName)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpFileData> GetFile(string fileGuid)
    {
        if (string.IsNullOrWhiteSpace(fileGuid))
        {
            throw new ResponseException(StatusCodes.Status400BadRequest, "File name not provided");
        }

        (string fileName, string fileGuid, string contentType)? entry = storedFiles.Where(tuple => tuple.fileGuid == fileGuid).SingleOrDefault();
        if (!entry.HasValue)
        {
            throw new ResponseException(StatusCodes.Status404NotFound, "File not found");
        }

        return new(entry.Value.fileName, await GetFileData(entry.Value.fileGuid), entry.Value.contentType);
    }

    public async Task<FileGuid> SaveFile(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            throw new ResponseException(StatusCodes.Status400BadRequest, "Parameters not set");
        }

        (string fileName, string fileGuid, string contentType) newEntry = (file.FileName, BuildUniqueFileName(file.FileName), file.ContentType);
        Directory.SetCurrentDirectory(DirectoryPaths.UserData);
        using (FileStream fs = File.Create(newEntry.fileGuid))
        {
            await file.CopyToAsync(fs);
        }
        Directory.SetCurrentDirectory(DirectoryPaths.Parent);
        storedFiles.Add(newEntry);
        return new FileGuid(newEntry.fileGuid);
    }

    private async Task<byte[]> GetFileData(string fileGuid)
    {
        Directory.SetCurrentDirectory(DirectoryPaths.UserData);
        byte[] data = await File.ReadAllBytesAsync(fileGuid);
        Directory.SetCurrentDirectory(DirectoryPaths.Parent);
        return data;
    }

    private string BuildUniqueFileName(string fileName) => Guid.NewGuid().ToString() + "." + fileName.Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[1];
}
