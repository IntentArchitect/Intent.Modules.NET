using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Delete.CustomResponseDefaultWithResponse
{
    public class CustomResponseDefaultWithResponse : IRequest<string>, ICommand
    {
        public CustomResponseDefaultWithResponse()
        {
        }
    }
}