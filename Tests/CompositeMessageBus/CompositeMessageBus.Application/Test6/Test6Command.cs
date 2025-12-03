using CompositeMessageBus.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CompositeMessageBus.Application.Test6
{
    public class Test6Command : IRequest, ICommand
    {
        public Test6Command(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}