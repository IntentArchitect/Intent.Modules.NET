using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Put.CustomResponse202WithResponse
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomResponse202WithResponseHandler : IRequestHandler<CustomResponse202WithResponse, string>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse202WithResponseHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<string> Handle(CustomResponse202WithResponse request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CustomResponse202WithResponseHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}