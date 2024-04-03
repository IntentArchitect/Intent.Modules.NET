using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories.MappableStoredProcs;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.GetMockEntities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetMockEntitiesHandler : IRequestHandler<GetMockEntities, List<MockEntityDto>>
    {
        private readonly IMockEntityRepository _mockEntityRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetMockEntitiesHandler(IMockEntityRepository mockEntityRepository, IMapper mapper)
        {
            _mockEntityRepository = mockEntityRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<MockEntityDto>> Handle(GetMockEntities request, CancellationToken cancellationToken)
        {
            var result = await _mockEntityRepository.GetMockEntities(cancellationToken);
            return result.MapToMockEntityDtoList(_mapper);
        }
    }
}