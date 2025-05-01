using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.OperationMapping;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users
{
    public class TaskItemDto : IMapFrom<TaskItem>
    {
        public TaskItemDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid TaskListId { get; set; }

        public static TaskItemDto Create(Guid id, string name, Guid taskListId)
        {
            return new TaskItemDto
            {
                Id = id,
                Name = name,
                TaskListId = taskListId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TaskItem, TaskItemDto>();
        }
    }
}