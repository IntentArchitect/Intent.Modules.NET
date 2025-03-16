using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.OperationMapping
{
    public class CreateUserWithTaskItemContractCommand
    {
        public CreateUserWithTaskItemContractCommand()
        {
            UserName = null!;
            ListName = null!;
            TaskName = null!;
            SubTasks = null!;
        }

        public string UserName { get; set; }
        public string ListName { get; set; }
        public string TaskName { get; set; }
        public List<CreateUserWithTaskItemContractCommandSubTasksDto> SubTasks { get; set; }

        public static CreateUserWithTaskItemContractCommand Create(
            string userName,
            string listName,
            string taskName,
            List<CreateUserWithTaskItemContractCommandSubTasksDto> subTasks)
        {
            return new CreateUserWithTaskItemContractCommand
            {
                UserName = userName,
                ListName = listName,
                TaskName = taskName,
                SubTasks = subTasks
            };
        }
    }
}