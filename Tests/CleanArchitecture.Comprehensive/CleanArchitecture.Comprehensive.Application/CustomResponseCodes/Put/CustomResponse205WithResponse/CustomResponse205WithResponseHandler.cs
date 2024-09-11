using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Put.CustomResponse205WithResponse
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomResponse205WithResponseHandler : IRequestHandler<CustomResponse205WithResponse, string>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse205WithResponseHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> Handle(CustomResponse205WithResponse request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CustomResponse205WithResponseHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}