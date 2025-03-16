using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping
{
    public class CreateUserWithTaskItemServiceSubTasksDto
    {
        public CreateUserWithTaskItemServiceSubTasksDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateUserWithTaskItemServiceSubTasksDto Create(string name)
        {
            return new CreateUserWithTaskItemServiceSubTasksDto
            {
                Name = name
            };
        }
    }
}