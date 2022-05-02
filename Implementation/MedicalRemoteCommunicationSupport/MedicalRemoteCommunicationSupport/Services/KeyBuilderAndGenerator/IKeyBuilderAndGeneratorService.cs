namespace MedicalRemoteCommunicationSupport.Services.KeyBuilderAndGenerator;

public interface IKeyBuilderAndGeneratorService
{
    Task<int> NextInSequance(string sequanceKey);
    string BuildRedisKey(string doctor, DateTime date);
    long ScoreFromTime(DateTime time);
}
