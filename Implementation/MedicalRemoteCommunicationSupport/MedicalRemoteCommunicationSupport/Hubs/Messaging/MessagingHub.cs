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
    private readonly IHandler<string, (DoctorRequestDto request, string specialization)> requestHandler;
    private readonly IHandler<string, ((string fullName, string skypeId) pattientAdditionalData, MyConnection connection)> requestAcceptedHandler;
    private readonly IHandler<string, RequestRejectionData> requestRejectedHandler;

    public MessagingHub(IConnectionManager connectionManager,
        IHandler<Message, Message> messageHandler,
        IHandler<string, (DoctorRequestDto request, string specialization)> requestHandler,
        IHandler<string, ((string fullName, string skypeId) pattientAdditionalData, MyConnection connection)> requestAcceptedHandler,
        IHandler<string, RequestRejectionData> requestRejectedHandler)
    {
        this.connectionManager = connectionManager;
        this.messageHandler = messageHandler;
        this.requestHandler = requestHandler;
        this.requestAcceptedHandler = requestAcceptedHandler;
        this.requestRejectedHandler = requestRejectedHandler;
    }

    public override async Task OnConnectedAsync()
    {
        await connectionManager.RegisterConnection(Context.UserIdentifier, Context.ConnectionId);
        Console.WriteLine($"{Context.UserIdentifier} has connected.");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await connectionManager.RemoveConnection(Context.UserIdentifier);
        Console.WriteLine($"{Context.UserIdentifier} has disconnected.");
        await base.OnDisconnectedAsync(exception);
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
        var data = await requestHandler.Handle(doctor, new[] { Context.UserIdentifier });
        if (await connectionManager.GetConnectionId(doctor) is string connectionId && connectionId != string.Empty)
            await Clients.Client(connectionId).RequestReceived(data.request.Username, data.request.FullName);
        await Clients.Caller.RequestSent(doctor, data.specialization);
    }

    [HubMethodName("acceptRequest")]
    public async Task AcceptRequest(string patient)
    {
        var data = await requestAcceptedHandler.Handle(patient, new[] { Context.UserIdentifier });
        if (await connectionManager.GetConnectionId(patient) is string connectionId && connectionId != string.Empty)
            await Clients.Client(connectionId).RequestFinished(data.connection.Username, data.connection.FullName,
                data.connection.SkypeId);
        await Clients.Caller.RequestAccepted(patient, data.pattientAdditionalData.fullName,
            data.pattientAdditionalData.skypeId);
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
