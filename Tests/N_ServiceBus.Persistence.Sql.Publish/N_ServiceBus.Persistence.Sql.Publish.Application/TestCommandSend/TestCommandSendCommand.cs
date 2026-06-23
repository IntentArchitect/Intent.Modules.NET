using Intent.RoslynWeaver.Attributes;
using MediatR;
using N_ServiceBus.Persistence.Sql.Publish.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace N_ServiceBus.Persistence.Sql.Publish.Application.TestCommandSend
{
    public class TestCommandSendCommand : IRequest, ICommand
    {
        public TestCommandSendCommand(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}