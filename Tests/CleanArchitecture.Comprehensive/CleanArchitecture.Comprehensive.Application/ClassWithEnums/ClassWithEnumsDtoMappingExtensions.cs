using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.Enums;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ClassWithEnums
{
    public static class ClassWithEnumsDtoMappingExtensions
    {
        public static ClassWithEnumsDto MapToClassWithEnumsDto(this Domain.Entities.Enums.ClassWithEnums projectFrom, IMapper mapper)
            => mapper.Map<ClassWithEnumsDto>(projectFrom);

        public static List<ClassWithEnumsDto> MapToClassWithEnumsDtoList(this IEnumerable<Domain.Entities.Enums.ClassWithEnums> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToClassWithEnumsDto(mapper)).ToList();
    }
}