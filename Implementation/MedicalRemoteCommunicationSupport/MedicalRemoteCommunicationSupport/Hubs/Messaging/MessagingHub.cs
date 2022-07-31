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
    private readonly IHandler<string, (string patientFullName, MyConnection connection)> requestAcceptedHandler;
    private readonly IHandler<string, RequestRejectionData> requestRejectedHandler;

    public MessagingHub(IConnectionManager connectionManager,
        IHandler<Message, Message> messageHandler,
        IHandler<string,RequestDto> requestHandler,
        IHandler<string, (string patientFullName, MyConnection connection)> requestAcceptedHandler,
        IHandler<string, RequestRejectionData> requestRejectedHandler)
    {
        this.connectionManager = connectionManager;
        this.messageHandler = messageHandler;
        this.requestHandler = requestHandler;
        this.requestAcceptedHandler = requestAcceptedHandler;
        this.requestRejectedHandler = requestRejectedHandler;
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
        await Clients.Caller.ReceiveMessage(Context.UserIdentifier, content, message.TimeSent);
    }

    [HubMethodName("sendRequest")]
    public async Task SendRequest(string doctor)
    {
        var request = await requestHandler.Handle(doctor, new[] { Context.UserIdentifier });
        if (await connectionManager.GetConnectionId(doctor) is string connectionId && connectionId != string.Empty)
            await Clients.Client(connectionId).RequestReceived(request.Username, request.FullName);
        await Clients.Caller.RequestSent(doctor);
    }

    [HubMethodName("acceptRequest")]
    public async Task AcceptRequest(string patient)
    {
        var data = await requestAcceptedHandler.Handle(patient, new[] { Context.UserIdentifier });
        if (await connectionManager.GetConnectionId(patient) is string connectionId && connectionId != string.Empty)
            await Clients.Client(connectionId).RequestFinished(data.connection.Username, data.connection.FullName);
        await Clients.Caller.RequestAccepted(patient, data.patientFullName);
    }

    [HubMethodName("rejectRequest")]
    public async Task RejectRequest(string patient)
    {
        var rejectionData = await requestRejectedHandler.Handle(patient, new[] { Context.UserIdentifier });
        if (await connectionManager.GetConnectionId(patient) is string connectionId && connectionId != string.Empty)
            await Clients.Client(connectionId).RequestHasBeenRejected(Context.UserIdentifier, rejectionData.DoctorFullName);
        await Clients.Caller.RequestRejected(rejectionData.RejectedUsername);
    }
}
