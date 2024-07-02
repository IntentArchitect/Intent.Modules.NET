using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.HasMissingDeps
{
    public static class HasMissingDepDtoMappingExtensions
    {
        public static HasMissingDepDto MapToHasMissingDepDto(this HasMissingDep projectFrom, IMapper mapper)
            => mapper.Map<HasMissingDepDto>(projectFrom);

        public static List<HasMissingDepDto> MapToHasMissingDepDtoList(this IEnumerable<HasMissingDep> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToHasMissingDepDto(mapper)).ToList();
    }
}