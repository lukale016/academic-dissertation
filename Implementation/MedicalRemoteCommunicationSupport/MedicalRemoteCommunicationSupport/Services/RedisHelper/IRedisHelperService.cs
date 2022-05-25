namespace MedicalRemoteCommunicationSupport.Services;
public interface IRedisHelperService
{
    long ScoreFromTime(DateTime time);
    string BuildAppointmentKey(string doctor, DateTime date);
    string BuildChatKey(string user1, string user2);
}
