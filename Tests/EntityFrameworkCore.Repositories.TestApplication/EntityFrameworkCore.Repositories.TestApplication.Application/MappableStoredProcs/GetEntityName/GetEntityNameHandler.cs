using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories.MappableStoredProcs;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.GetEntityName
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEntityNameHandler : IRequestHandler<GetEntityName, string>
    {
        private readonly IMappableStoredProcRepository _mappableStoredProcRepository;

        [IntentManaged(Mode.Merge)]
        public GetEntityNameHandler(IMappableStoredProcRepository mappableStoredProcRepository)
        {
            _mappableStoredProcRepository = mappableStoredProcRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(GetEntityName request, CancellationToken cancellationToken)
        {
            var result = await _mappableStoredProcRepository.GetEntityName(request.Id, cancellationToken);
            return result;
        }
    }
}