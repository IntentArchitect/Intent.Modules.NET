using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Farmers
{
    public class AddPlotFarmerCommand
    {
        public AddPlotFarmerCommand()
        {
            Address = null!;
        }

        public Guid Id { get; set; }
        public AddPlotDto Address { get; set; }

        public static AddPlotFarmerCommand Create(Guid id, AddPlotDto address)
        {
            return new AddPlotFarmerCommand
            {
                Id = id,
                Address = address
            };
        }
    }
}