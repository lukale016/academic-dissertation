namespace MedicalRemoteCommunicationSupport.Services;

public interface IKeyGeneratorService
{
    Task<int> NextInSequence<T>();
}
