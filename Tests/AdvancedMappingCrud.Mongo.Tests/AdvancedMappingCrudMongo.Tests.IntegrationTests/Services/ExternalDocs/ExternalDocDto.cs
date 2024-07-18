using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.ExternalDocs
{
    public class ExternalDocDto
    {
        public ExternalDocDto()
        {
            Name = null!;
            Thing = null!;
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Thing { get; set; }

        public static ExternalDocDto Create(long id, string name, string thing)
        {
            return new ExternalDocDto
            {
                Id = id,
                Name = name,
                Thing = thing
            };
        }
    }
}