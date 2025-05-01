using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Users
{
    public class TaskItemDto
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
    }
}