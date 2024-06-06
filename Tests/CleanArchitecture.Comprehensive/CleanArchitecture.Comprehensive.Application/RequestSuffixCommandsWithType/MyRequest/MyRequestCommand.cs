using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.RequestSuffixCommandsWithType.MyRequest
{
    public class MyRequestCommand : IRequest, ICommand
    {
        public MyRequestCommand()
        {
        }
    }
}