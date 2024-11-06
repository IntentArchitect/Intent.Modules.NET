using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Hierarchy
{
    public static class Child1MappingExtensions
    {
        public static Child1 MapToChild1(this ChildSimple projectFrom, IMapper mapper)
            => mapper.Map<Child1>(projectFrom);

        public static List<Child1> MapToChild1List(this IEnumerable<ChildSimple> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToChild1(mapper)).ToList();
    }
}