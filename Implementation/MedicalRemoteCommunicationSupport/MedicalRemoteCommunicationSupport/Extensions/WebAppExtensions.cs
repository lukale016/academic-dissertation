using MedicalRemoteCommunicationSupport.Hubs.Messaging;

namespace MedicalRemoteCommunicationSupport.Extensions;

public static class WebAppExtensions
{
    private const string hubPrefix = "hub/";
    public static WebApplication MapHubs(this WebApplication app)
    {
        app.MapHub<MessagingHub>($"{hubPrefix}messaging");
        return app;
    }
}
