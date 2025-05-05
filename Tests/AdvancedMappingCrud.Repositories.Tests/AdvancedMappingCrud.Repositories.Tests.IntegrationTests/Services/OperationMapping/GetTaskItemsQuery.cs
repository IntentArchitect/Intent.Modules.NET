using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.OperationMapping
{
    public class GetTaskItemsQuery
    {
        public Guid UserId { get; set; }
        public Guid TaskListId { get; set; }

        public static GetTaskItemsQuery Create(Guid userId, Guid taskListId)
        {
            return new GetTaskItemsQuery
            {
                UserId = userId,
                TaskListId = taskListId
            };
        }
    }
}