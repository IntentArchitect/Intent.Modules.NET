using CompositeMessageBus.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CompositeMessageBus.Application.Test8
{
    public class Test8Command : IRequest, ICommand
    {
        public Test8Command(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}