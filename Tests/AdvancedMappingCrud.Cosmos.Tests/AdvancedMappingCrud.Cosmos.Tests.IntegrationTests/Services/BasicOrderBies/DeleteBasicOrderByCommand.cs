using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.BasicOrderBies
{
    public class DeleteBasicOrderByCommand
    {
        public DeleteBasicOrderByCommand()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public static DeleteBasicOrderByCommand Create(string id)
        {
            return new DeleteBasicOrderByCommand
            {
                Id = id
            };
        }
    }
}