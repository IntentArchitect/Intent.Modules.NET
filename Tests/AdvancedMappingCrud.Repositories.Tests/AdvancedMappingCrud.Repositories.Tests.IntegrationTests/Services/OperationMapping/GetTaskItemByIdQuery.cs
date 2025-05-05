using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.OperationMapping
{
    public class GetTaskItemByIdQuery
    {
        public Guid UserId { get; set; }
        public Guid TaskListId { get; set; }
        public Guid Id { get; set; }

        public static GetTaskItemByIdQuery Create(Guid userId, Guid taskListId, Guid id)
        {
            return new GetTaskItemByIdQuery
            {
                UserId = userId,
                TaskListId = taskListId,
                Id = id
            };
        }
    }
}