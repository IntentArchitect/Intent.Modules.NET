using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Farmers
{
    public class ChangeNameFarmerCommand
    {
        public ChangeNameFarmerCommand()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static ChangeNameFarmerCommand Create(Guid id, string name)
        {
            return new ChangeNameFarmerCommand
            {
                Id = id,
                Name = name
            };
        }
    }
}