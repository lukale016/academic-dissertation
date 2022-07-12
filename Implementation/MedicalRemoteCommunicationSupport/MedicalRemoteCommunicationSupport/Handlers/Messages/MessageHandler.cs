using Ardalis.GuardClauses;
using StackExchange.Redis;
using System.Text.Json;

namespace MedicalRemoteCommunicationSupport.Handlers;

public class MessageHandler : IHandler<Message>
{
    private readonly UnitOfWork unitOfWork;
    private readonly ConnectionMultiplexer redis;

    public MessageHandler(UnitOfWork unit, ConnectionMultiplexer redis)
    {
        this.unitOfWork = unit;
        this.redis = redis;
    }

    public async Task Handle(Message handled, params object[] additionalParams)
    {
        Guard.Against.Null(handled);
        Guard.Against.NullOrEmpty(handled.From);
        Guard.Against.NullOrEmpty(handled.Content);
        if (!additionalParams.Any())
            throw new ArgumentException("Sender is required as additional param", nameof(additionalParams));
        if(additionalParams[0] is not string)
            throw new ArgumentException("Sender should be first in param list", nameof(additionalParams));

        string receiverUsername = additionalParams[0] as string;
        try
        {
            Patient receiver = await unitOfWork.PatientRepository.GetUser(receiverUsername);
            Doctor sender = await unitOfWork.DoctorRepository.GetUser(handled.From);
            await redis.GetDatabase().ListLeftPushAsync(receiver.MessageKeyForUser(sender.Username), JsonSerializer.Serialize<Message>(handled));
        }
        catch(ResponseException ex)
        {
            if (!(ex.Status == StatusCodes.Status404NotFound) && !(ex.Status == StatusCodes.Status500InternalServerError))
                throw;
            Doctor receiver = await unitOfWork.DoctorRepository.GetUser(receiverUsername);
            Patient sender = await unitOfWork.PatientRepository.GetUser(handled.From);
            await redis.GetDatabase().ListLeftPushAsync(receiver.MessageKeyForUser(sender.Username), JsonSerializer.Serialize<Message>(handled));
        }
    }
}
