using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.People
{
    public class PersonCreateDto
    {
        public PersonCreateDto()
        {
            Details = null!;
        }

        public PersonCreatePersonPersonDetailsDto Details { get; set; }

        public static PersonCreateDto Create(PersonCreatePersonPersonDetailsDto details)
        {
            return new PersonCreateDto
            {
                Details = details
            };
        }
    }
}