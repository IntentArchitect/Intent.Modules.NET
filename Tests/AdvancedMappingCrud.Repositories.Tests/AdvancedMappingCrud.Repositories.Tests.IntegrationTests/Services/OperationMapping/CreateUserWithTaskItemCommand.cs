using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.OperationMapping
{
    public class CreateUserWithTaskItemCommand
    {
        public CreateUserWithTaskItemCommand()
        {
            UserName = null!;
            ListName = null!;
            TaskName = null!;
            SubTasks = null!;
        }

        public string UserName { get; set; }
        public string ListName { get; set; }
        public string TaskName { get; set; }
        public List<CreateUserWithTaskItemCommandSubTasksDto> SubTasks { get; set; }

        public static CreateUserWithTaskItemCommand Create(
            string userName,
            string listName,
            string taskName,
            List<CreateUserWithTaskItemCommandSubTasksDto> subTasks)
        {
            return new CreateUserWithTaskItemCommand
            {
                UserName = userName,
                ListName = listName,
                TaskName = taskName,
                SubTasks = subTasks
            };
        }
    }
}