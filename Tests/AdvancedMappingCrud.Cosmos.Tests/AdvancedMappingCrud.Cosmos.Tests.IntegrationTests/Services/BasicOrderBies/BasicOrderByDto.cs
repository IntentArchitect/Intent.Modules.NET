using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.BasicOrderBies
{
    public class BasicOrderByDto
    {
        public BasicOrderByDto()
        {
            Id = null!;
            Name = null!;
            Surname = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public static BasicOrderByDto Create(string id, string name, string surname)
        {
            return new BasicOrderByDto
            {
                Id = id,
                Name = name,
                Surname = surname
            };
        }
    }
}