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

namespace IntegrationTesting.Tests.Application.DiffIds.GetDiffIds
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDiffIdsQueryHandler : IRequestHandler<GetDiffIdsQuery, List<DiffIdDto>>
    {
        private readonly IDiffIdRepository _diffIdRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetDiffIdsQueryHandler(IDiffIdRepository diffIdRepository, IMapper mapper)
        {
            _diffIdRepository = diffIdRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<DiffIdDto>> Handle(GetDiffIdsQuery request, CancellationToken cancellationToken)
        {
            var diffIds = await _diffIdRepository.FindAllAsync(cancellationToken);
            return diffIds.MapToDiffIdDtoList(_mapper);
        }
    }
}