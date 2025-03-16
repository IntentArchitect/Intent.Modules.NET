using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.OperationMapping
{
    public class CreateUserWithTaskItemContractCommandSubTasksDto
    {
        public CreateUserWithTaskItemContractCommandSubTasksDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateUserWithTaskItemContractCommandSubTasksDto Create(string name)
        {
            return new CreateUserWithTaskItemContractCommandSubTasksDto
            {
                Name = name
            };
        }
    }
}