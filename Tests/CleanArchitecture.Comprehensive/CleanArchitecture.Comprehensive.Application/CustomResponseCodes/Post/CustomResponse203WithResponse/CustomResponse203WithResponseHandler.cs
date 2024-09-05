using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse203WithResponse
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomResponse203WithResponseHandler : IRequestHandler<CustomResponse203WithResponse, string>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse203WithResponseHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> Handle(CustomResponse203WithResponse request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CustomResponse203WithResponseHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}