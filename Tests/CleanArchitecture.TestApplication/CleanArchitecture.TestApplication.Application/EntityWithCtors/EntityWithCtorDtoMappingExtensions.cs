using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.EntityWithCtors
{
    public static class EntityWithCtorDtoMappingExtensions
    {
        public static EntityWithCtorDto MapToEntityWithCtorDto(this EntityWithCtor projectFrom, IMapper mapper)
        {
            return mapper.Map<EntityWithCtorDto>(projectFrom);
        }

        public static List<EntityWithCtorDto> MapToEntityWithCtorDtoList(this IEnumerable<EntityWithCtor> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToEntityWithCtorDto(mapper)).ToList();
        }
    }
}