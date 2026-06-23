using Intent.RoslynWeaver.Attributes;
using MediatR;
using N_ServiceBus.Persistence.Sql.Publish.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace N_ServiceBus.Persistence.Sql.Publish.Application.TestEventSend
{
    public class TestEventSendCommand : IRequest, ICommand
    {
        public TestEventSendCommand(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}