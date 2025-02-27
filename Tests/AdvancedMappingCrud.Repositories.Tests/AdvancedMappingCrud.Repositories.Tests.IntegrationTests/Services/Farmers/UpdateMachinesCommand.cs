using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Farmers
{
    public class UpdateMachinesCommand
    {
        public UpdateMachinesCommand()
        {
            Name = null!;
        }

        public Guid FarmerId { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }

        public static UpdateMachinesCommand Create(Guid farmerId, string name, Guid id)
        {
            return new UpdateMachinesCommand
            {
                FarmerId = farmerId,
                Name = name,
                Id = id
            };
        }
    }
}