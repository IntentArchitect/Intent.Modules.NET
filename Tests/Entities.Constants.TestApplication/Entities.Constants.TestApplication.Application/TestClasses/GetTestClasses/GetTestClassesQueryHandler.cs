using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entities.Constants.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Entities.Constants.TestApplication.Application.TestClasses.GetTestClasses
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetTestClassesQueryHandler : IRequestHandler<GetTestClassesQuery, List<TestClassDto>>
    {
        private readonly ITestClassRepository _testClassRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetTestClassesQueryHandler(ITestClassRepository testClassRepository, IMapper mapper)
        {
            _testClassRepository = testClassRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<TestClassDto>> Handle(GetTestClassesQuery request, CancellationToken cancellationToken)
        {
            var testClasses = await _testClassRepository.FindAllAsync(cancellationToken);
            return testClasses.MapToTestClassDtoList(_mapper);
        }
    }
}