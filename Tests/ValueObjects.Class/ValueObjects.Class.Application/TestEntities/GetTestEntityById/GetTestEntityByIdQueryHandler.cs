using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ValueObjects.Class.Domain.Common.Exceptions;
using ValueObjects.Class.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace ValueObjects.Class.Application.TestEntities.GetTestEntityById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetTestEntityByIdQueryHandler : IRequestHandler<GetTestEntityByIdQuery, TestEntityDto>
    {
        private readonly ITestEntityRepository _testEntityRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetTestEntityByIdQueryHandler(ITestEntityRepository testEntityRepository, IMapper mapper)
        {
            _testEntityRepository = testEntityRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<TestEntityDto> Handle(GetTestEntityByIdQuery request, CancellationToken cancellationToken)
        {
            var testEntity = await _testEntityRepository.FindByIdAsync(request.Id, cancellationToken);
            if (testEntity is null)
            {
                throw new NotFoundException($"Could not find TestEntity '{request.Id}'");
            }
            return testEntity.MapToTestEntityDto(_mapper);
        }
    }
}