using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse207WithResponse
{
    public class CustomResponse207WithResponse : IRequest<string>, ICommand
    {
        public CustomResponse207WithResponse()
        {
        }
    }
}