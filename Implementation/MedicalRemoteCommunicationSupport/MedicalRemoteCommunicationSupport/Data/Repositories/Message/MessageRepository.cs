using Ardalis.GuardClauses;
using StackExchange.Redis;
using System.Text.Json;

namespace MedicalRemoteCommunicationSupport.Data.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly UnitOfWork unitOfWork;
    private readonly ConnectionMultiplexer redis;

    public MessageRepository(UnitOfWork unit, ConnectionMultiplexer redis)
    {
        this.unitOfWork = unit;
        this.redis = redis;
    }

    public async Task<IEnumerable<Message>> GetMessages(string sender, string receiver)
    {
        Guard.Against.NullOrEmpty(sender, nameof(sender));
        Guard.Against.NullOrEmpty(receiver, nameof(receiver));

        UserBase receiverUser = null;
        UserBase senderUser = null;

        try
        {
            receiverUser = await unitOfWork.PatientRepository.GetUser(receiver);
            senderUser = await unitOfWork.DoctorRepository.GetUser(sender);
        }
        catch (ResponseException ex)
        {
            if (!(ex.Status == StatusCodes.Status404NotFound) && !(ex.Status == StatusCodes.Status500InternalServerError))
                throw;
            receiverUser = await unitOfWork.DoctorRepository.GetUser(receiver);
            senderUser = await unitOfWork.PatientRepository.GetUser(sender);
        }

        if (receiverUser is null || senderUser is null)
            return Enumerable.Empty<Message>();

        IDatabase db = redis.GetDatabase();
        IEnumerable<Message> receivedMessages = (await db.ListRangeAsync(receiverUser.MessageKeyForUser(sender)))
                                                .Select(rv => JsonSerializer.Deserialize<Message>(rv));
        IEnumerable<Message> sentMessages = (await db.ListRangeAsync(senderUser.MessageKeyForUser(receiver)))
                                                .Select(rv => JsonSerializer.Deserialize<Message>(rv));
        if (receivedMessages is null && sentMessages is null)
            return Enumerable.Empty<Message>();
        if(receivedMessages is null)
            receivedMessages = Enumerable.Empty<Message>();
        if(sentMessages is null)
            sentMessages = Enumerable.Empty<Message>();

        return receivedMessages.Union(sentMessages).OrderBy(m => m.TimeSent);
    }

    public async Task<Message> StoreMessage(string receiver, Message message)
    {
        Guard.Against.NullOrEmpty(receiver, nameof(receiver));
        Guard.Against.Null(message, nameof(message));
        Guard.Against.NullOrEmpty(message.From, nameof(message.From));

        try
        {
            Patient receiverUser = await unitOfWork.PatientRepository.GetUser(receiver);
            Doctor senderUser = await unitOfWork.DoctorRepository.GetUser(message.From);
            await redis.GetDatabase().ListLeftPushAsync(receiverUser.MessageKeyForUser(senderUser.Username), JsonSerializer.Serialize<Message>(message));
        }
        catch (ResponseException ex)
        {
            if (!(ex.Status == StatusCodes.Status404NotFound) && !(ex.Status == StatusCodes.Status500InternalServerError))
                throw;
            Doctor receiverUser = await unitOfWork.DoctorRepository.GetUser(receiver);
            Patient senderUser = await unitOfWork.PatientRepository.GetUser(message.From);
            await redis.GetDatabase().ListLeftPushAsync(receiverUser.MessageKeyForUser(senderUser.Username), JsonSerializer.Serialize<Message>(message));
        }

        return message;
    }
}
