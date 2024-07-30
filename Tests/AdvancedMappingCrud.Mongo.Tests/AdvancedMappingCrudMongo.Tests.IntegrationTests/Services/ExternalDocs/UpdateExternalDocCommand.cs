using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.ExternalDocs
{
    public class UpdateExternalDocCommand
    {
        public UpdateExternalDocCommand()
        {
            Name = null!;
            Thing = null!;
        }

        public string Name { get; set; }
        public string Thing { get; set; }
        public long Id { get; set; }

        public static UpdateExternalDocCommand Create(string name, string thing, long id)
        {
            return new UpdateExternalDocCommand
            {
                Name = name,
                Thing = thing,
                Id = id
            };
        }
    }
}