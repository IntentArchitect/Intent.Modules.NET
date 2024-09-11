using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Put.CustomResponse206WithResponse
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomResponse206WithResponseHandler : IRequestHandler<CustomResponse206WithResponse, string>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse206WithResponseHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> Handle(CustomResponse206WithResponse request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CustomResponse206WithResponseHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}