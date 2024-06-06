using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Repositories.Nullability;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.TestNullablities.GetTestNullabilityWithNullReturn
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetTestNullabilityWithNullReturnHandler : IRequestHandler<GetTestNullabilityWithNullReturn, TestNullablityDto?>
    {
        private readonly ITestNullablityRepository _testNullablityRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetTestNullabilityWithNullReturnHandler(ITestNullablityRepository testNullablityRepository, IMapper mapper)
        {
            _testNullablityRepository = testNullablityRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<TestNullablityDto?> Handle(
            GetTestNullabilityWithNullReturn request,
            CancellationToken cancellationToken)
        {
            var testNullablity = await _testNullablityRepository.FindByIdAsync(request.Id, cancellationToken);
            if (testNullablity is null)
            {
                return null;
            }

            return testNullablity.MapToTestNullablityDto(_mapper);
        }
    }
}