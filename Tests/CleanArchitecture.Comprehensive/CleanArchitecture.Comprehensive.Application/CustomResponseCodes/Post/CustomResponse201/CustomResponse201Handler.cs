using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse201
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomResponse201Handler : IRequestHandler<CustomResponse201>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse201Handler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(CustomResponse201 request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CustomResponse201Handler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}