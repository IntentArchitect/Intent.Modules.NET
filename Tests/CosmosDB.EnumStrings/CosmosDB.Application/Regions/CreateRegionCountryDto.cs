using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.Application.Regions
{
    public class CreateRegionCountryDto
    {
        public CreateRegionCountryDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateRegionCountryDto Create(string name)
        {
            return new CreateRegionCountryDto
            {
                Name = name
            };
        }
    }
}