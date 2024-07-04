using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Orders
{
    public class GetOrdersByRefNoQuery
    {
        public string? RefNo { get; set; }
        public string? ExternalRefNo { get; set; }

        public static GetOrdersByRefNoQuery Create(string? refNo, string? externalRefNo)
        {
            return new GetOrdersByRefNoQuery
            {
                RefNo = refNo,
                ExternalRefNo = externalRefNo
            };
        }
    }
}