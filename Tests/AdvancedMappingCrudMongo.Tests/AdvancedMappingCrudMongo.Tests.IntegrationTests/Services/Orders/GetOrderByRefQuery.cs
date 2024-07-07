using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Orders
{
    public class GetOrderByRefQuery
    {
        public string? RefNo { get; set; }
        public string? ExternalRefNo { get; set; }

        public static GetOrderByRefQuery Create(string? refNo, string? externalRefNo)
        {
            return new GetOrderByRefQuery
            {
                RefNo = refNo,
                ExternalRefNo = externalRefNo
            };
        }
    }
}