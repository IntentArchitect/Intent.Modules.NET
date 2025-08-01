using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Patch.CustomResponse208WithResponse
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomResponse208WithResponseHandler : IRequestHandler<CustomResponse208WithResponse, string>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse208WithResponseHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<string> Handle(CustomResponse208WithResponse request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CustomResponse208WithResponseHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}