namespace MedicalRemoteCommunicationSupport.Records;

public record HttpFileData(string Name, byte[] Data, string ContentType);