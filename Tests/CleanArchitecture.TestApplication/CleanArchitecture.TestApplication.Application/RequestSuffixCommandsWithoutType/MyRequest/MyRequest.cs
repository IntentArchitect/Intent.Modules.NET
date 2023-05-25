using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.RequestSuffixCommandsWithoutType.MyRequest
{
    public class MyRequest : IRequest, ICommand
    {
        public MyRequest()
        {
        }
    }
}