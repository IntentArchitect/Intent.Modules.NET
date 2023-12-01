using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.SignalR;
using SignalR.Application;
using SignalR.Application.Interfaces;
using SignalR.Application.Interfaces.Hubs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.SignalR.HubService", Version = "1.0")]

namespace SignalR.Api.Hubs.Services
{
    public class NotificationsHubService : INotificationsHub
    {
        private readonly IHubContext<NotificationsHub> _hub;

        public NotificationsHubService(IHubContext<NotificationsHub> hub)
        {
            _hub = hub;
        }

        public async Task SendAsync(MessageToAllDto model)
        {
            await _hub.Clients.All.SendAsync("MessageToAllDto", model);
        }

        public async Task SendAsync(MessageToGroupDto model, string groupId)
        {
            await _hub.Clients.Group(groupId).SendAsync("MessageToGroupDto", model);
        }

        public async Task SendAsync(MessageToGroupsDto model, IReadOnlyList<string> groupIds)
        {
            await _hub.Clients.Groups(groupIds).SendAsync("MessageToGroupsDto", model);
        }

        public async Task SendAsync(MessageToUserDto model, string userId)
        {
            await _hub.Clients.User(userId).SendAsync("MessageToUserDto", model);
        }

        public async Task SendAsync(MessageToUsersDto model, IReadOnlyList<string> userIds)
        {
            await _hub.Clients.Users(userIds).SendAsync("MessageToUsersDto", model);
        }

        public async Task SendAsync(MassageToClientDto model, string connectionId)
        {
            await _hub.Clients.Client(connectionId).SendAsync("MassageToClientDto", model);
        }
    }
}