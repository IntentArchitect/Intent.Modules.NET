using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping
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