using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Repositories.Inheritance;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.GetConcreteClasses
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetConcreteClassesQueryHandler : IRequestHandler<GetConcreteClassesQuery, List<ConcreteClassDto>>
    {
        private readonly IConcreteClassRepository _concreteClassRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetConcreteClassesQueryHandler(IConcreteClassRepository concreteClassRepository, IMapper mapper)
        {
            _concreteClassRepository = concreteClassRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ConcreteClassDto>> Handle(
            GetConcreteClassesQuery request,
            CancellationToken cancellationToken)
        {
            var concreteClasses = await _concreteClassRepository.FindAllAsync(cancellationToken);
            return concreteClasses.MapToConcreteClassDtoList(_mapper);
        }
    }
}