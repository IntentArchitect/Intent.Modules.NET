using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.HasMissingDeps.GetHasMissingDeps
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetHasMissingDepsQueryHandler : IRequestHandler<GetHasMissingDepsQuery, List<HasMissingDepDto>>
    {
        private readonly IHasMissingDepRepository _hasMissingDepRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetHasMissingDepsQueryHandler(IHasMissingDepRepository hasMissingDepRepository, IMapper mapper)
        {
            _hasMissingDepRepository = hasMissingDepRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<HasMissingDepDto>> Handle(
            GetHasMissingDepsQuery request,
            CancellationToken cancellationToken)
        {
            var hasMissingDeps = await _hasMissingDepRepository.FindAllAsync(cancellationToken);
            return hasMissingDeps.MapToHasMissingDepDtoList(_mapper);
        }
    }
}