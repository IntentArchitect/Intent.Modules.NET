using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.ClassAS
{
    public static class ClassADTOMappingExtensions
    {
        public static ClassADTO MapToClassADTO(this ClassA projectFrom, IMapper mapper)
        {
            return mapper.Map<ClassADTO>(projectFrom);
        }

        public static List<ClassADTO> MapToClassADTOList(this IEnumerable<ClassA> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToClassADTO(mapper)).ToList();
        }
    }
}