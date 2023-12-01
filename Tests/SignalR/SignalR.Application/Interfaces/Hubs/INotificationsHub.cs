using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.SignalR.HubServiceInterface", Version = "1.0")]

namespace SignalR.Application.Interfaces.Hubs
{
    public interface INotificationsHub
    {
        Task SendAsync(MessageToAllDto model);
        Task SendAsync(MessageToGroupDto model, string groupId);
        Task SendAsync(MessageToGroupsDto model, IReadOnlyList<string> groupIds);
        Task SendAsync(MessageToUserDto model, string userId);
        Task SendAsync(MessageToUsersDto model, IReadOnlyList<string> userIds);
        Task SendAsync(MassageToClientDto model, string connectionId);
    }
}