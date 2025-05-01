using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.OperationMapping
{
    public class UpdateTaskItemCommand
    {
        public UpdateTaskItemCommand()
        {
            Name = null!;
        }

        public Guid UserId { get; set; }
        public Guid TaskListId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }

        public static UpdateTaskItemCommand Create(Guid userId, Guid taskListId, Guid id, string name)
        {
            return new UpdateTaskItemCommand
            {
                UserId = userId,
                TaskListId = taskListId,
                Id = id,
                Name = name
            };
        }
    }
}