using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Farmers
{
    public class GetFarmerByIdQuery
    {
        public Guid Id { get; set; }

        public static GetFarmerByIdQuery Create(Guid id)
        {
            return new GetFarmerByIdQuery
            {
                Id = id
            };
        }
    }
}