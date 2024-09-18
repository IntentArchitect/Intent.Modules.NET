using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CosmosDB.Application.DerivedOfTS.GetDerivedOfTS
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDerivedOfTSQueryHandler : IRequestHandler<GetDerivedOfTSQuery, List<DerivedOfTDto>>
    {
        private readonly IDerivedOfTRepository _derivedOfTRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetDerivedOfTSQueryHandler(IDerivedOfTRepository derivedOfTRepository, IMapper mapper)
        {
            _derivedOfTRepository = derivedOfTRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<DerivedOfTDto>> Handle(GetDerivedOfTSQuery request, CancellationToken cancellationToken)
        {
            var derivedOfTs = await _derivedOfTRepository.FindAllAsync(cancellationToken);
            return derivedOfTs.MapToDerivedOfTDtoList(_mapper);
        }
    }
}