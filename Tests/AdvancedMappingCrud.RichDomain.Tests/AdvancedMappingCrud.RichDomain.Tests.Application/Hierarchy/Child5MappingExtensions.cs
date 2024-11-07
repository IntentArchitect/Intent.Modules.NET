using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Hierarchy
{
    public static class Child5MappingExtensions
    {
        public static Child5 MapToChild5(this FamilyComplex projectFrom, IMapper mapper)
            => mapper.Map<Child5>(projectFrom);

        public static List<Child5> MapToChild5List(this IEnumerable<FamilyComplex> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToChild5(mapper)).ToList();
    }
}