using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Basics
{
    public class DeleteBasicCommand
    {
        public DeleteBasicCommand()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public static DeleteBasicCommand Create(string id)
        {
            return new DeleteBasicCommand
            {
                Id = id
            };
        }
    }
}