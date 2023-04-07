using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.EntityWithMutableOperations
{
    public static class EntityWithMutableOperationDtoMappingExtensions
    {
        public static EntityWithMutableOperationDto MapToEntityWithMutableOperationDto(this EntityWithMutableOperation projectFrom, IMapper mapper)
        {
            return mapper.Map<EntityWithMutableOperationDto>(projectFrom);
        }

        public static List<EntityWithMutableOperationDto> MapToEntityWithMutableOperationDtoList(this IEnumerable<EntityWithMutableOperation> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToEntityWithMutableOperationDto(mapper)).ToList();
        }
    }
}