using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Farmers
{
    public class DeleteMachinesCommand
    {
        public Guid FarmerId { get; set; }
        public Guid Id { get; set; }

        public static DeleteMachinesCommand Create(Guid farmerId, Guid id)
        {
            return new DeleteMachinesCommand
            {
                FarmerId = farmerId,
                Id = id
            };
        }
    }
}