using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Get.CustomResponse207WithResponse
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomResponse207WithResponseHandler : IRequestHandler<CustomResponse207WithResponse, string>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse207WithResponseHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> Handle(CustomResponse207WithResponse request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CustomResponse207WithResponseHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}