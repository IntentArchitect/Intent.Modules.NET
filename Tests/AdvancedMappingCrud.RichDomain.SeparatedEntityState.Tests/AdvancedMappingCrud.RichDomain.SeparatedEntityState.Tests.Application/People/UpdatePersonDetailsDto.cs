using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.People
{
    public class UpdatePersonDetailsDto
    {
        public UpdatePersonDetailsDto()
        {
            Name = null!;
        }

        public UpdatePersonDetailsNameDto Name { get; set; }

        public static UpdatePersonDetailsDto Create(UpdatePersonDetailsNameDto name)
        {
            return new UpdatePersonDetailsDto
            {
                Name = name
            };
        }
    }
}