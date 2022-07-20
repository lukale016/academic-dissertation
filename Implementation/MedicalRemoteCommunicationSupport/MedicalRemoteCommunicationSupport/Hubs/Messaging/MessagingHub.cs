using MedicalRemoteCommunicationSupport.Handlers;
using MedicalRemoteCommunicationSupport.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MedicalRemoteCommunicationSupport.Hubs.Messaging;

[Authorize]
public class MessagingHub: Hub<IClientMethods>
{
    private readonly IConnectionManager connectionManager;
    private readonly IHandler<Message> messageHandler;

    public MessagingHub(IConnectionManager connectionManager, IHandler<Message> messageHandler)
    {
        this.connectionManager = connectionManager;
        this.messageHandler = messageHandler;
    }

    public override Task OnConnectedAsync()
    {
        connectionManager.RegisterConnection(Context.UserIdentifier, Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        connectionManager.RemoveConnection(Context.UserIdentifier);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string receiver, string content)
    {
        var message = new Message { From = Context.UserIdentifier, Content = content, TimeSent = DateTime.Now };
        await messageHandler.Handle(message, new[] { receiver });
        if(await connectionManager.GetConnectionId(receiver) is string connectionId && connectionId != string.Empty)
            await Clients.Client(connectionId).ReceiveMessage(Context.UserIdentifier, content, message.TimeSent);
    }
}
