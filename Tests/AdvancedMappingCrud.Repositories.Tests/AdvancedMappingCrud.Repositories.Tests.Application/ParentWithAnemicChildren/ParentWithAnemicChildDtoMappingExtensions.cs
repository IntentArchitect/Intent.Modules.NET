using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.AnemicChild;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren
{
    public static class ParentWithAnemicChildDtoMappingExtensions
    {
        public static ParentWithAnemicChildDto MapToParentWithAnemicChildDto(this ParentWithAnemicChild projectFrom, IMapper mapper)
            => mapper.Map<ParentWithAnemicChildDto>(projectFrom);

        public static List<ParentWithAnemicChildDto> MapToParentWithAnemicChildDtoList(this IEnumerable<ParentWithAnemicChild> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToParentWithAnemicChildDto(mapper)).ToList();
    }
}