using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.SimpleOdata
{
    public class DeleteSimpleOdataCommand
    {
        public DeleteSimpleOdataCommand()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public static DeleteSimpleOdataCommand Create(string id)
        {
            return new DeleteSimpleOdataCommand
            {
                Id = id
            };
        }
    }
}