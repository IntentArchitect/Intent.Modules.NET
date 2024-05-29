using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.Nullability;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.TestNullablities.GetTestNullablityById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetTestNullablityByIdQueryHandler : IRequestHandler<GetTestNullablityByIdQuery, TestNullablityDto>
    {
        private readonly ITestNullablityRepository _testNullablityRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetTestNullablityByIdQueryHandler(ITestNullablityRepository testNullablityRepository, IMapper mapper)
        {
            _testNullablityRepository = testNullablityRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<TestNullablityDto> Handle(
            GetTestNullablityByIdQuery request,
            CancellationToken cancellationToken)
        {
            var testNullablity = await _testNullablityRepository.FindByIdAsync(request.Id, cancellationToken);
            if (testNullablity is null)
            {
                throw new NotFoundException($"Could not find TestNullablity '{request.Id}'");
            }

            return testNullablity.MapToTestNullablityDto(_mapper);
        }
    }
}