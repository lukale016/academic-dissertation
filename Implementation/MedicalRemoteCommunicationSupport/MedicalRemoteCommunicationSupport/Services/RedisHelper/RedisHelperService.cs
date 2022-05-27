namespace MedicalRemoteCommunicationSupport.Services;
public static class RedisHelperService
{
    private const string dateFormat = "dd.MM.yyyy";
    private static DateTime referenceDate = new DateTime(2022, 1, 1);

    public static string BuildAppointmentKey(string user, DateTime date)
    {
        return $"Appointment:{user}_{date.ToString(dateFormat)}";
    }

    public static string BuildChatKey(string user1, string user2)
    {
        throw new NotImplementedException();
    }

    public static long ScoreFromTime(DateTime time)
    {
        return referenceDate.Ticks - time.Ticks;
    }


}
