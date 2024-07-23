using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Basics
{
    public class UpdateBasicCommand
    {
        public UpdateBasicCommand()
        {
            Name = null!;
            Id = null!;
        }

        public string Name { get; set; }
        public string Id { get; set; }

        public static UpdateBasicCommand Create(string name, string id)
        {
            return new UpdateBasicCommand
            {
                Name = name,
                Id = id
            };
        }
    }
}