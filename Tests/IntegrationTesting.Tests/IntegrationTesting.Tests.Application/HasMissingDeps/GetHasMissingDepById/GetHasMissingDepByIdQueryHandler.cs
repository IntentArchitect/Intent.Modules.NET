using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.HasMissingDeps.GetHasMissingDepById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetHasMissingDepByIdQueryHandler : IRequestHandler<GetHasMissingDepByIdQuery, HasMissingDepDto>
    {
        private readonly IHasMissingDepRepository _hasMissingDepRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetHasMissingDepByIdQueryHandler(IHasMissingDepRepository hasMissingDepRepository, IMapper mapper)
        {
            _hasMissingDepRepository = hasMissingDepRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<HasMissingDepDto> Handle(GetHasMissingDepByIdQuery request, CancellationToken cancellationToken)
        {
            var hasMissingDep = await _hasMissingDepRepository.FindByIdAsync(request.Id, cancellationToken);
            if (hasMissingDep is null)
            {
                throw new NotFoundException($"Could not find HasMissingDep '{request.Id}'");
            }
            return hasMissingDep.MapToHasMissingDepDto(_mapper);
        }
    }
}