using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Farmers
{
    public class ChangeNameMachinesCommand
    {
        public ChangeNameMachinesCommand()
        {
            Name = null!;
        }

        public Guid FarmerId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }

        public static ChangeNameMachinesCommand Create(Guid farmerId, Guid id, string name)
        {
            return new ChangeNameMachinesCommand
            {
                FarmerId = farmerId,
                Id = id,
                Name = name
            };
        }
    }
}