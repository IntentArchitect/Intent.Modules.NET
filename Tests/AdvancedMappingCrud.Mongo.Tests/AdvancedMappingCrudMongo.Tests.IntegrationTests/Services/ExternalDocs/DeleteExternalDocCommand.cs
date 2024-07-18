using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.ExternalDocs
{
    public class DeleteExternalDocCommand
    {
        public long Id { get; set; }

        public static DeleteExternalDocCommand Create(long id)
        {
            return new DeleteExternalDocCommand
            {
                Id = id
            };
        }
    }
}