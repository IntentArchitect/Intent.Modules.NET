using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping
{
    public class CreateUserWithTaskItemCommandSubTasksDto
    {
        public CreateUserWithTaskItemCommandSubTasksDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateUserWithTaskItemCommandSubTasksDto Create(string name)
        {
            return new CreateUserWithTaskItemCommandSubTasksDto
            {
                Name = name
            };
        }
    }
}