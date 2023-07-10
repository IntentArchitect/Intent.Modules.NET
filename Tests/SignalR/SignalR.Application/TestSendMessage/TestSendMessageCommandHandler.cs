using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SignalR.Application.Interfaces.Hubs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace SignalR.Application.TestSendMessage
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestSendMessageCommandHandler : IRequestHandler<TestSendMessageCommand>
    {
        private readonly INotificationsHub _hub;

        [IntentManaged(Mode.Merge)]
        public TestSendMessageCommandHandler(INotificationsHub hub)
        {
            _hub = hub;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Unit> Handle(TestSendMessageCommand request, CancellationToken cancellationToken)
        {
            await _hub.SendAsync(new MessageToAllDto { Message = request.Message });
            await _hub.SendAsync(new MessageToUserDto { Message = request.Message }, "");
            await _hub.SendAsync(new MessageToUsersDto { Message = request.Message }, new[] { "" });
            await _hub.SendAsync(new MessageToGroupDto { Message = request.Message }, "");
            await _hub.SendAsync(new MessageToGroupsDto { Message = request.Message }, new[] { "" });
            return Unit.Value;
        }
    }
}