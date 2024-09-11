using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponseDefaultWithResponse
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomResponseDefaultWithResponseHandler : IRequestHandler<CustomResponseDefaultWithResponse, string>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponseDefaultWithResponseHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> Handle(CustomResponseDefaultWithResponse request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CustomResponseDefaultWithResponseHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}