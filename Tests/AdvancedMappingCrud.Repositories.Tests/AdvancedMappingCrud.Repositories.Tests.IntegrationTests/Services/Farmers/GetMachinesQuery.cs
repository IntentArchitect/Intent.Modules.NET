using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Farmers
{
    public class GetMachinesQuery
    {
        public Guid FarmerId { get; set; }

        public static GetMachinesQuery Create(Guid farmerId)
        {
            return new GetMachinesQuery
            {
                FarmerId = farmerId
            };
        }
    }
}