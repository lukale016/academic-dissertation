using Ardalis.GuardClauses;
using StackExchange.Redis;
using System.Text.Json;

namespace MedicalRemoteCommunicationSupport.Handlers;

public class MessageHandler : IHandler<Message, Message>
{
    private readonly UnitOfWork unitOfWork;

    public MessageHandler(UnitOfWork unit)
    {
        this.unitOfWork = unit;
    }

    public async Task<Message> Handle(Message handled, params object[] additionalParams)
    {
        Guard.Against.Null(handled);
        Guard.Against.NullOrEmpty(handled.From);
        Guard.Against.NullOrEmpty(handled.Content);
        if (!additionalParams.Any())
            throw new ArgumentException("Receiver is required as additional param", nameof(additionalParams));
        if(additionalParams[0] is not string)
            throw new ArgumentException("Receiver should be first in param list", nameof(additionalParams));

        string receiver = additionalParams[0] as string;
        await unitOfWork.MessageRepository.StoreMessage(receiver, handled);

        return handled;
    }
}
