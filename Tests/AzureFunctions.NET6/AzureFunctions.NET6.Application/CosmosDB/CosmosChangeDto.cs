using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AzureFunctions.NET6.Application.CosmosDB
{
    public class CosmosChangeDto
    {
        public CosmosChangeDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CosmosChangeDto Create(string name)
        {
            return new CosmosChangeDto
            {
                Name = name
            };
        }
    }
}