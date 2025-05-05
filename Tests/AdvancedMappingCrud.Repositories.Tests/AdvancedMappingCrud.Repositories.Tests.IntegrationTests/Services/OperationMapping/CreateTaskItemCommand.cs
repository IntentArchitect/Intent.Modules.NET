using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.OperationMapping
{
    public class CreateTaskItemCommand
    {
        public CreateTaskItemCommand()
        {
            Name = null!;
        }

        public Guid UserId { get; set; }
        public Guid TaskListId { get; set; }
        public string Name { get; set; }

        public static CreateTaskItemCommand Create(Guid userId, Guid taskListId, string name)
        {
            return new CreateTaskItemCommand
            {
                UserId = userId,
                TaskListId = taskListId,
                Name = name
            };
        }
    }
}