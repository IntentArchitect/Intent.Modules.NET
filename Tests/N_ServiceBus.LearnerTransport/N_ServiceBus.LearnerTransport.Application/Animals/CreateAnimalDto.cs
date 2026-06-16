using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace N_ServiceBus.LearnerTransport.Application.Animals
{
    public record CreateAnimalDto
    {
        public CreateAnimalDto()
        {
            Name = null!;
            Type = null!;
        }

        public string Name { get; init; }
        public string Type { get; init; }
    }
}