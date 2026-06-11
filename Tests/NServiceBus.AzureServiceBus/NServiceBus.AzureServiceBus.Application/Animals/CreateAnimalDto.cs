using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace NServiceBus.AzureServiceBus.Application.Animals
{
    public record CreateAnimalDto
    {
        public CreateAnimalDto()
        {
            Name = null!;
        }

        public string Name { get; init; }
    }
}