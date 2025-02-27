using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Farmers
{
    public class MachinesDto
    {
        public MachinesDto()
        {
            Name = null!;
        }

        public Guid FarmerId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }

        public static MachinesDto Create(Guid farmerId, Guid id, string name)
        {
            return new MachinesDto
            {
                FarmerId = farmerId,
                Id = id,
                Name = name
            };
        }
    }
}