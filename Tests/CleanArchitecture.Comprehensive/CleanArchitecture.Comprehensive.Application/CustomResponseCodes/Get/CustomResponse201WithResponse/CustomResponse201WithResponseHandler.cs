using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Get.CustomResponse201WithResponse
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomResponse201WithResponseHandler : IRequestHandler<CustomResponse201WithResponse, string>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse201WithResponseHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> Handle(CustomResponse201WithResponse request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CustomResponse201WithResponseHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}