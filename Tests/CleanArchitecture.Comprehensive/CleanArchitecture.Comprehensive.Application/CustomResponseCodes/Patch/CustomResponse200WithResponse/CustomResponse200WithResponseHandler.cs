using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Patch.CustomResponse200WithResponse
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomResponse200WithResponseHandler : IRequestHandler<CustomResponse200WithResponse, string>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse200WithResponseHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> Handle(CustomResponse200WithResponse request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CustomResponse200WithResponseHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}