using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entities.Constants.TestApplication.Domain.Common.Exceptions;
using Entities.Constants.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Entities.Constants.TestApplication.Application.TestClasses.GetTestClassById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetTestClassByIdQueryHandler : IRequestHandler<GetTestClassByIdQuery, TestClassDto>
    {
        private readonly ITestClassRepository _testClassRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetTestClassByIdQueryHandler(ITestClassRepository testClassRepository, IMapper mapper)
        {
            _testClassRepository = testClassRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<TestClassDto> Handle(GetTestClassByIdQuery request, CancellationToken cancellationToken)
        {
            var testClass = await _testClassRepository.FindByIdAsync(request.Id, cancellationToken);

            if (testClass is null)
            {
                throw new NotFoundException($"Could not find TestClass '{request.Id}'");
            }

            return testClass.MapToTestClassDto(_mapper);
        }
    }
}