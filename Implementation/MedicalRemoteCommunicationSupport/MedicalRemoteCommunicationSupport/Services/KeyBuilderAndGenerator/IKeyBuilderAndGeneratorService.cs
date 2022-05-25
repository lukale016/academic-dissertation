namespace MedicalRemoteCommunicationSupport.Services;

public interface IKeyBuilderAndGeneratorService
{
    Task<int> NextInSequence(string sequenceKey);
}
