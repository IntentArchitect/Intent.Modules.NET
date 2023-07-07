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

        public async Task PublishSendToAllDtoAsync(SendToAllDto model)
        {
            await _hub.Clients.All.SendAsync("SendToAllDto", model);
        }

        public async Task PublishSendToUserDtoAsync(string userId, SendToUserDto model)
        {
            await _hub.Clients.User(userId).SendAsync("SendToUserDto", model);
        }

        public async Task PublishSendToUsersDtoAsync(IReadOnlyList<string> userIds, SendToUsersDto model)
        {
            await _hub.Clients.Users(userIds).SendAsync("SendToUsersDto", model);
        }

        public async Task PublishSendToGroupDtoAsync(string groupId, SendToGroupDto model)
        {
            await _hub.Clients.Group(groupId).SendAsync("SendToGroupDto", model);
        }

        public async Task PublishSendToGroupsDtoAsync(IReadOnlyList<string> groupIds, SendToGroupsDto model)
        {
            await _hub.Clients.Groups(groupIds).SendAsync("SendToGroupsDto", model);
        }
    }
}