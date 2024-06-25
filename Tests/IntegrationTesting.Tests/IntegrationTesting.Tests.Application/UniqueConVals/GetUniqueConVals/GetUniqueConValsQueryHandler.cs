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

namespace IntegrationTesting.Tests.Application.UniqueConVals.GetUniqueConVals
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetUniqueConValsQueryHandler : IRequestHandler<GetUniqueConValsQuery, List<UniqueConValDto>>
    {
        private readonly IUniqueConValRepository _uniqueConValRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetUniqueConValsQueryHandler(IUniqueConValRepository uniqueConValRepository, IMapper mapper)
        {
            _uniqueConValRepository = uniqueConValRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<UniqueConValDto>> Handle(GetUniqueConValsQuery request, CancellationToken cancellationToken)
        {
            var uniqueConVals = await _uniqueConValRepository.FindAllAsync(cancellationToken);
            return uniqueConVals.MapToUniqueConValDtoList(_mapper);
        }
    }
}