using Intent.RoslynWeaver.Attributes;
using MediatR;
using SignalR.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace SignalR.Application.TestSendMessage
{
    public class TestSendMessageCommand : IRequest, ICommand
    {
        public TestSendMessageCommand(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}