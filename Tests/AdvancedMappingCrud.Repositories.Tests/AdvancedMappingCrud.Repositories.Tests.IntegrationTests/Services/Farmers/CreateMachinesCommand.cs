using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Farmers
{
    public class CreateMachinesCommand
    {
        public CreateMachinesCommand()
        {
            Name = null!;
        }

        public Guid FarmerId { get; set; }
        public string Name { get; set; }

        public static CreateMachinesCommand Create(Guid farmerId, string name)
        {
            return new CreateMachinesCommand
            {
                FarmerId = farmerId,
                Name = name
            };
        }
    }
}