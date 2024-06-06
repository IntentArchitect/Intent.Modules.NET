using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.Inheritance;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.GetConcreteClassById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetConcreteClassByIdQueryHandler : IRequestHandler<GetConcreteClassByIdQuery, ConcreteClassDto>
    {
        private readonly IConcreteClassRepository _concreteClassRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetConcreteClassByIdQueryHandler(IConcreteClassRepository concreteClassRepository, IMapper mapper)
        {
            _concreteClassRepository = concreteClassRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ConcreteClassDto> Handle(GetConcreteClassByIdQuery request, CancellationToken cancellationToken)
        {
            var concreteClass = await _concreteClassRepository.FindByIdAsync(request.Id, cancellationToken);
            if (concreteClass is null)
            {
                throw new NotFoundException($"Could not find ConcreteClass '{request.Id}'");
            }

            return concreteClass.MapToConcreteClassDto(_mapper);
        }
    }
}