namespace MedicalRemoteCommunicationSupport.Services;

public class FileManager : IFileManager
{
    /// <summary>
    /// Item1 - File name
    /// Item2 - Unique file name
    /// Item3 - Content type
    /// </summary>
    private List<(string, string, string)> storedFiles = new();

    public void DeleteFile(string fileName)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpFileData> GetFile(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ResponseException(StatusCodes.Status400BadRequest, "File name not provided");
        }

        (string, string, string)? entry = storedFiles.Where(tuple => tuple.Item1 == fileName).SingleOrDefault();
        if (!entry.HasValue)
        {
            throw new ResponseException(StatusCodes.Status404NotFound, "File not found");
        }

        return new(entry.Value.Item1, await GetFileData(entry.Value.Item2), entry.Value.Item3);
    }

    public async Task SaveFile(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            throw new ResponseException(StatusCodes.Status400BadRequest, "Parameters not set");
        }

        (string, string, string) newEntry = (file.FileName, BuildUniqueFileName(file.FileName), file.ContentType);
        Directory.SetCurrentDirectory(DirectoryPaths.UserData);
        using (FileStream fs = File.Create(newEntry.Item2))
        {
            await file.CopyToAsync(fs);
        }
        Directory.SetCurrentDirectory(DirectoryPaths.Parent);
        storedFiles.Add(newEntry);
    }

    private async Task<byte[]> GetFileData(string fileName)
    {
        Directory.SetCurrentDirectory(DirectoryPaths.UserData);
        byte[] data = await File.ReadAllBytesAsync(fileName);
        Directory.SetCurrentDirectory(DirectoryPaths.Parent);
        return data;
    }

    private string BuildUniqueFileName(string fileName) => Guid.NewGuid().ToString() + "." + fileName.Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[1];
}
