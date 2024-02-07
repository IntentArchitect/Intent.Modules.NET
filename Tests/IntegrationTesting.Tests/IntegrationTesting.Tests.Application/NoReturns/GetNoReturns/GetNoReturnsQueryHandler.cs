using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.NoReturns.GetNoReturns
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetNoReturnsQueryHandler : IRequestHandler<GetNoReturnsQuery, List<NoReturnDto>>
    {
        private readonly INoReturnRepository _noReturnRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetNoReturnsQueryHandler(INoReturnRepository noReturnRepository, IMapper mapper)
        {
            _noReturnRepository = noReturnRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<NoReturnDto>> Handle(GetNoReturnsQuery request, CancellationToken cancellationToken)
        {
            var noReturns = await _noReturnRepository.FindAllAsync(cancellationToken);
            return noReturns.MapToNoReturnDtoList(_mapper);
        }
    }
}