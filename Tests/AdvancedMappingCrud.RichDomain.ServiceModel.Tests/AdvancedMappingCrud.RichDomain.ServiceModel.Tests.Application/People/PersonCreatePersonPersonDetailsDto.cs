using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.People
{
    public class PersonCreatePersonPersonDetailsDto
    {
        public PersonCreatePersonPersonDetailsDto()
        {
            Name = null!;
        }

        public PersonCreatePersonPersonDetailsNameDto Name { get; set; }

        public static PersonCreatePersonPersonDetailsDto Create(PersonCreatePersonPersonDetailsNameDto name)
        {
            return new PersonCreatePersonPersonDetailsDto
            {
                Name = name
            };
        }
    }
}