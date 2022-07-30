using MedicalRemoteCommunicationSupport.Handlers;
using MedicalRemoteCommunicationSupport.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MedicalRemoteCommunicationSupport.Hubs.Messaging;

[Authorize]
public class MessagingHub: Hub<IClientMethods>
{
    private readonly IConnectionManager connectionManager;
    private readonly IHandler<Message, Message> messageHandler;
    private readonly IHandler<string, RequestDto> requestHandler;
    private readonly IHandler<string, MyConnection> requestAcceptedHandler;

    public MessagingHub(IConnectionManager connectionManager,
        IHandler<Message, Message> messageHandler,
        IHandler<string,RequestDto> requestHandler,
        IHandler<string, MyConnection> requestAcceptedHandler)
    {
        this.connectionManager = connectionManager;
        this.messageHandler = messageHandler;
        this.requestHandler = requestHandler;
        this.requestAcceptedHandler = requestAcceptedHandler;
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

    [HubMethodName("sendMessage")]
    public async Task SendMessage(string receiver, string content)
    {
        var message = new Message { From = Context.UserIdentifier, Content = content, TimeSent = DateTime.Now };
        await messageHandler.Handle(message, new[] { receiver });
        if(await connectionManager.GetConnectionId(receiver) is string connectionId && connectionId != string.Empty)
            await Clients.Client(connectionId).ReceiveMessage(Context.UserIdentifier, content, message.TimeSent);
        await Clients.Caller.ReceiveMessage(receiver, content, message.TimeSent);
    }

    [HubMethodName("sendRequest")]
    public async Task SendRequest(string doctor)
    {
        var request = await requestHandler.Handle(doctor, new[] { Context.UserIdentifier });
        if (await connectionManager.GetConnectionId(doctor) is string connectionId && connectionId != string.Empty)
            await Clients.Client(connectionId).RequestReceived(request.Username, request.FullName);
    }

    [HubMethodName("acceptRequest")]
    public async Task AcceptRequest(string patient)
    {
        var connection = await requestAcceptedHandler.Handle(patient, new[] { Context.UserIdentifier });
        if (await connectionManager.GetConnectionId(patient) is string connectionId && connectionId != string.Empty)
            await Clients.Client(connectionId).RequestAccepted(connection.Username, connection.FullName);
    }
}
