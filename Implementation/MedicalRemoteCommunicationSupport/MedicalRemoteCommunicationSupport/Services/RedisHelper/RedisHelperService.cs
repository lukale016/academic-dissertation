namespace MedicalRemoteCommunicationSupport.Services;
public class RedisHelperService: IRedisHelperService
{
    private string dateFormat = "dd-MM-yyyy";
    private DateTime referenceDate = new DateTime(2022, 1, 1);

    public string BuildAppointmentKey(string doctor, DateTime date)
    {
        return $"{doctor}_{date.ToString(dateFormat)}";
    }

    public string BuildChatKey(string user1, string user2)
    {
        throw new NotImplementedException();
    }

    public long ScoreFromTime(DateTime time)
    {
        return referenceDate.Ticks - time.Ticks;
    }


}
