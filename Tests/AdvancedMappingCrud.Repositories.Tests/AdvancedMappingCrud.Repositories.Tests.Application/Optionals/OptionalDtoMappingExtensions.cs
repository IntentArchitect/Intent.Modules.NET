using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Optionals
{
    public static class OptionalDtoMappingExtensions
    {
        public static OptionalDto MapToOptionalDto(this Optional projectFrom, IMapper mapper)
            => mapper.Map<OptionalDto>(projectFrom);

        public static List<OptionalDto> MapToOptionalDtoList(this IEnumerable<Optional> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOptionalDto(mapper)).ToList();
    }
}