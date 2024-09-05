using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Get.CustomResponse204
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomResponse204Handler : IRequestHandler<CustomResponse204>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse204Handler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(CustomResponse204 request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CustomResponse204Handler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}