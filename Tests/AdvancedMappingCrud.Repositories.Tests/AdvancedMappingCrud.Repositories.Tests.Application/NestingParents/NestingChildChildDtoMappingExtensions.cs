using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.MappingTests;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.NestingParents
{
    public static class NestingChildChildDtoMappingExtensions
    {
        public static NestingChildChildDto MapToNestingChildChildDto(this NestingChildChild projectFrom, IMapper mapper)
            => mapper.Map<NestingChildChildDto>(projectFrom);

        public static List<NestingChildChildDto> MapToNestingChildChildDtoList(this IEnumerable<NestingChildChild> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToNestingChildChildDto(mapper)).ToList();
    }
}