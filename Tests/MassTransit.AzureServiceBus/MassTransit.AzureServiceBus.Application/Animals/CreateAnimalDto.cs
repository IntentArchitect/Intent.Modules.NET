using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Application.Animals
{
    public class CreateAnimalDto
    {
        public CreateAnimalDto()
        {
            Name = null!;
            Type = null!;
        }

        public string Name { get; set; }
        public string Type { get; set; }

        public static CreateAnimalDto Create(string name, string type)
        {
            return new CreateAnimalDto
            {
                Name = name,
                Type = type
            };
        }
    }
}