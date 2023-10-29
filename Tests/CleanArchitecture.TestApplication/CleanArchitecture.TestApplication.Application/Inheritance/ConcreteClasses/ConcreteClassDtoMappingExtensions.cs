using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Entities.Inheritance;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Inheritance.ConcreteClasses
{
    public static class ConcreteClassDtoMappingExtensions
    {
        public static ConcreteClassDto MapToConcreteClassDto(this ConcreteClass projectFrom, IMapper mapper)
            => mapper.Map<ConcreteClassDto>(projectFrom);

        public static List<ConcreteClassDto> MapToConcreteClassDtoList(this IEnumerable<ConcreteClass> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToConcreteClassDto(mapper)).ToList();
    }
}