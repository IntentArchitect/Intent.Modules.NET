using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.People
{
    public class UpdateUpdatePersonDetailsDto
    {
        public UpdateUpdatePersonDetailsDto()
        {
            Name = null!;
        }

        public UpdateUpdatePersonDetailsNameDto Name { get; set; }

        public static UpdateUpdatePersonDetailsDto Create(UpdateUpdatePersonDetailsNameDto name)
        {
            return new UpdateUpdatePersonDetailsDto
            {
                Name = name
            };
        }
    }
}