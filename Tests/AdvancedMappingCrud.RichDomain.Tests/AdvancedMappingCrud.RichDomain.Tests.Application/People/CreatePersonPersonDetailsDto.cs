using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.People
{
    public class CreatePersonPersonDetailsDto
    {
        public CreatePersonPersonDetailsDto()
        {
            Name = null!;
        }

        public CreatePersonPersonDetailsNameDto Name { get; set; }

        public static CreatePersonPersonDetailsDto Create(CreatePersonPersonDetailsNameDto name)
        {
            return new CreatePersonPersonDetailsDto
            {
                Name = name
            };
        }
    }
}