using CompositeMessageBus.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CompositeMessageBus.Application.Test7
{
    public class Test7Command : IRequest, ICommand
    {
        public Test7Command(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}