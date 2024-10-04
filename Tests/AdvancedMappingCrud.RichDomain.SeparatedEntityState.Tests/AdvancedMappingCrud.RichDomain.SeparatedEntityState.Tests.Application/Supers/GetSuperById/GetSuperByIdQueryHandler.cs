using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Supers.GetSuperById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetSuperByIdQueryHandler : IRequestHandler<GetSuperByIdQuery, SuperDto>
    {
        private readonly ISuperRepository _superRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetSuperByIdQueryHandler(ISuperRepository superRepository, IMapper mapper)
        {
            _superRepository = superRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<SuperDto> Handle(GetSuperByIdQuery request, CancellationToken cancellationToken)
        {
            var super = await _superRepository.FindByIdAsync(request.Id, cancellationToken);
            if (super is null)
            {
                throw new NotFoundException($"Could not find Super '{request.Id}'");
            }
            return super.MapToSuperDto(_mapper);
        }
    }
}