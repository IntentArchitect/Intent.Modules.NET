using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping
{
    public class CreateUserWithTaskItemContractServiceSubTasksDto
    {
        public CreateUserWithTaskItemContractServiceSubTasksDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateUserWithTaskItemContractServiceSubTasksDto Create(string name)
        {
            return new CreateUserWithTaskItemContractServiceSubTasksDto
            {
                Name = name
            };
        }
    }
}