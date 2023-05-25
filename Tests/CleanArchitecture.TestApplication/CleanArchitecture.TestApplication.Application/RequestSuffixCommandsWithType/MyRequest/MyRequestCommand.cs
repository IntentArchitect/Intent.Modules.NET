using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.RequestSuffixCommandsWithType.MyRequest
{
    public class MyRequestCommand : IRequest, ICommand
    {
        public MyRequestCommand()
        {
        }
    }
}