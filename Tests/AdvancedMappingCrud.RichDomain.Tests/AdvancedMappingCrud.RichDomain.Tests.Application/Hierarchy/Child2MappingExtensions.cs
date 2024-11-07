using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Hierarchy
{
    public static class Child2MappingExtensions
    {
        public static Child2 MapToChild2(this ChildParentExcluded projectFrom, IMapper mapper)
            => mapper.Map<Child2>(projectFrom);

        public static List<Child2> MapToChild2List(this IEnumerable<ChildParentExcluded> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToChild2(mapper)).ToList();
    }
}