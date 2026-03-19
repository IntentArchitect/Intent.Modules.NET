using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ServiceDispatch.NoToken
{
    public class NoTokenCommand : IRequest, ICommand
    {
        public NoTokenCommand(int input)
        {
            Input = input;
        }

        public int Input { get; set; }
    }
}