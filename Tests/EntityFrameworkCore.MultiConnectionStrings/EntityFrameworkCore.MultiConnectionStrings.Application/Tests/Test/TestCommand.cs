using EntityFrameworkCore.MultiConnectionStrings.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EntityFrameworkCore.MultiConnectionStrings.Application.Tests.Test
{
    public class TestCommand : IRequest, ICommand
    {
        public TestCommand(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}