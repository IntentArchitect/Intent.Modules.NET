using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories.MappableStoredProcs;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.MismatchType
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MismatchTypeQueryHandler : IRequestHandler<MismatchTypeQuery, int>
    {
        private readonly IMappableStoredProcRepository _mappableStoredProcRepository;

        [IntentManaged(Mode.Merge)]
        public MismatchTypeQueryHandler(IMappableStoredProcRepository mappableStoredProcRepository)
        {
            _mappableStoredProcRepository = mappableStoredProcRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<int> Handle(MismatchTypeQuery request, CancellationToken cancellationToken)
        {
            var result = await _mappableStoredProcRepository.GetEntityById(request.Id, cancellationToken);

            // TODO: Implement return type mapping...
            throw new NotImplementedException("Implement return type mapping...");
        }
    }
}