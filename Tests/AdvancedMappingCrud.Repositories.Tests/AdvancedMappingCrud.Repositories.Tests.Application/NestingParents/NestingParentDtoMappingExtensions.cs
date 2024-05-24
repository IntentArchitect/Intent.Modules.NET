using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.MappingTests;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.NestingParents
{
    public static class NestingParentDtoMappingExtensions
    {
        public static NestingParentDto MapToNestingParentDto(this NestingParent projectFrom, IMapper mapper)
            => mapper.Map<NestingParentDto>(projectFrom);

        public static List<NestingParentDto> MapToNestingParentDtoList(this IEnumerable<NestingParent> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToNestingParentDto(mapper)).ToList();
    }
}