using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Delete.CustomResponse226WithResponse
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomResponse226WithResponseHandler : IRequestHandler<CustomResponse226WithResponse, string>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse226WithResponseHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> Handle(CustomResponse226WithResponse request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CustomResponse226WithResponseHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}