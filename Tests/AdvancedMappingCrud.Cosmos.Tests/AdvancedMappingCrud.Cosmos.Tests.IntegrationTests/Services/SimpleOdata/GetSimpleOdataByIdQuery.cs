using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.SimpleOdata
{
    public class GetSimpleOdataByIdQuery
    {
        public GetSimpleOdataByIdQuery()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public static GetSimpleOdataByIdQuery Create(string id)
        {
            return new GetSimpleOdataByIdQuery
            {
                Id = id
            };
        }
    }
}