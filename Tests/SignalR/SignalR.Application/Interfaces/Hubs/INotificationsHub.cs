using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.SignalR.HubServiceInterface", Version = "1.0")]

namespace SignalR.Application.Interfaces.Hubs
{
    public interface INotificationsHub
    {
        Task PublishSendToAllDtoAsync(SendToAllDto model);
        Task PublishSendToUserDtoAsync(string userId, SendToUserDto model);
        Task PublishSendToUsersDtoAsync(IReadOnlyList<string> userIds, SendToUsersDto model);
        Task PublishSendToGroupDtoAsync(string groupId, SendToGroupDto model);
        Task PublishSendToGroupsDtoAsync(IReadOnlyList<string> groupIds, SendToGroupsDto model);
    }
}