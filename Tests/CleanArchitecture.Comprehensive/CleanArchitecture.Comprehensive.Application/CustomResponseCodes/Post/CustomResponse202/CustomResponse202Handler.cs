using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse202
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomResponse202Handler : IRequestHandler<CustomResponse202>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse202Handler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(CustomResponse202 request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CustomResponse202Handler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}