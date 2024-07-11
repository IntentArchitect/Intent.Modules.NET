using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.SimpleOdata
{
    public class SimpleOdataDto
    {
        public SimpleOdataDto()
        {
            Id = null!;
            Name = null!;
            Surname = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public static SimpleOdataDto Create(string id, string name, string surname)
        {
            return new SimpleOdataDto
            {
                Id = id,
                Name = name,
                Surname = surname
            };
        }
    }
}