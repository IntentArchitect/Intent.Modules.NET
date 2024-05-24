using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ValueObjects.Class.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace ValueObjects.Class.Application.TestEntities.GetTestEntities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetTestEntitiesQueryHandler : IRequestHandler<GetTestEntitiesQuery, List<TestEntityDto>>
    {
        private readonly ITestEntityRepository _testEntityRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetTestEntitiesQueryHandler(ITestEntityRepository testEntityRepository, IMapper mapper)
        {
            _testEntityRepository = testEntityRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<TestEntityDto>> Handle(GetTestEntitiesQuery request, CancellationToken cancellationToken)
        {
            var testEntities = await _testEntityRepository.FindAllAsync(cancellationToken);
            return testEntities.MapToTestEntityDtoList(_mapper);
        }
    }
}