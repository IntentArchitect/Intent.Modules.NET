using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.VariantTypesClasses
{
    public static class VariantTypesClassDtoMappingExtensions
    {
        public static VariantTypesClassDto MapToVariantTypesClassDto(this VariantTypesClass projectFrom, IMapper mapper)
        {
            return mapper.Map<VariantTypesClassDto>(projectFrom);
        }

        public static List<VariantTypesClassDto> MapToVariantTypesClassDtoList(this IEnumerable<VariantTypesClass> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToVariantTypesClassDto(mapper)).ToList();
        }
    }
}