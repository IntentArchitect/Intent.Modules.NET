using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.SignalR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.SignalR.Hub", Version = "1.0")]

namespace SignalR.Api.Hubs
{
    public class NotificationsHub : Hub
    {
    }
}