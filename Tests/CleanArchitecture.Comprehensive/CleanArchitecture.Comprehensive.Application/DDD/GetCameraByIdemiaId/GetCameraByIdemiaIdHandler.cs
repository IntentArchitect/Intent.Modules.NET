using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD.GetCameraByIdemiaId
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCameraByIdemiaIdHandler : IRequestHandler<GetCameraByIdemiaId, GetCameraDto>
    {
        [IntentManaged(Mode.Merge)]
        public GetCameraByIdemiaIdHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<GetCameraDto> Handle(GetCameraByIdemiaId request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetCameraByIdemiaIdHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}