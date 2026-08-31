using CompositeMessageBus.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CompositeMessageBus.Application.Test10
{
    public class Test10Command : IRequest, ICommand
    {
        public Test10Command(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}