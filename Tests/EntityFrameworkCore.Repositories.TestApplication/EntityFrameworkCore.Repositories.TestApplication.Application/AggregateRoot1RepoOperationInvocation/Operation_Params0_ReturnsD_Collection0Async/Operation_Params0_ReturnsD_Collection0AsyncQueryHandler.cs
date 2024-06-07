using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Application.CommonDtos;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.AggregateRoot1RepoOperationInvocation.Operation_Params0_ReturnsD_Collection0Async
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class Operation_Params0_ReturnsD_Collection0AsyncQueryHandler : IRequestHandler<Operation_Params0_ReturnsD_Collection0AsyncQuery, SpResultDto>
    {
        private readonly IAggregateRoot1Repository _aggregateRoot1Repository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public Operation_Params0_ReturnsD_Collection0AsyncQueryHandler(IAggregateRoot1Repository aggregateRoot1Repository,
            IMapper mapper)
        {
            _aggregateRoot1Repository = aggregateRoot1Repository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<SpResultDto> Handle(
            Operation_Params0_ReturnsD_Collection0AsyncQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _aggregateRoot1Repository.Operation_Params0_ReturnsD_Collection0Async(cancellationToken);
            return result.MapToSpResultDto(_mapper);
        }
    }
}