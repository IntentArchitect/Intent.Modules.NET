using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Repositories.Nullability;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.TestNullablities.GetTestNullablities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetTestNullablitiesQueryHandler : IRequestHandler<GetTestNullablitiesQuery, List<TestNullablityDto>>
    {
        private readonly ITestNullablityRepository _testNullablityRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetTestNullablitiesQueryHandler(ITestNullablityRepository testNullablityRepository, IMapper mapper)
        {
            _testNullablityRepository = testNullablityRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<TestNullablityDto>> Handle(
            GetTestNullablitiesQuery request,
            CancellationToken cancellationToken)
        {
            var testNullablities = await _testNullablityRepository.FindAllAsync(cancellationToken);
            return testNullablities.MapToTestNullablityDtoList(_mapper);
        }
    }
}