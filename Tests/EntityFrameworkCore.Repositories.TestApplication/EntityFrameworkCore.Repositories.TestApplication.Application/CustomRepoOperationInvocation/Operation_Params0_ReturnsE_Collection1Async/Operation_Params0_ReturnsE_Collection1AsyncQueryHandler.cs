using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Application.CommonDtos;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.CustomRepoOperationInvocation.Operation_Params0_ReturnsE_Collection1Async
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class Operation_Params0_ReturnsE_Collection1AsyncQueryHandler : IRequestHandler<Operation_Params0_ReturnsE_Collection1AsyncQuery, List<AggregateRoot1Dto>>
    {
        private readonly ICustomRepository _customRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public Operation_Params0_ReturnsE_Collection1AsyncQueryHandler(ICustomRepository customRepository, IMapper mapper)
        {
            _customRepository = customRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<AggregateRoot1Dto>> Handle(
            Operation_Params0_ReturnsE_Collection1AsyncQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _customRepository.Operation_Params0_ReturnsE_Collection1Async(cancellationToken);
            return result.MapToAggregateRoot1DtoList(_mapper);
        }
    }
}