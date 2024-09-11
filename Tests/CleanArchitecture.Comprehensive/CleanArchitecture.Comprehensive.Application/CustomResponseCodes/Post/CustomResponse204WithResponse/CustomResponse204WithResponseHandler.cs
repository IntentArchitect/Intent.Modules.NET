using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse204WithResponse
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomResponse204WithResponseHandler : IRequestHandler<CustomResponse204WithResponse, string>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse204WithResponseHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> Handle(CustomResponse204WithResponse request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CustomResponse204WithResponseHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}