using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.RequestSuffixCommandsWithType.MyCommandRequest
{
    public class MyCommandRequest : IRequest, ICommand
    {
        public MyCommandRequest()
        {
        }
    }
}