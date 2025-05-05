using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.OperationMapping;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users
{
    public static class TaskItemDtoMappingExtensions
    {
        public static TaskItemDto MapToTaskItemDto(this TaskItem projectFrom, IMapper mapper)
            => mapper.Map<TaskItemDto>(projectFrom);

        public static List<TaskItemDto> MapToTaskItemDtoList(this IEnumerable<TaskItem> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToTaskItemDto(mapper)).ToList();
    }
}