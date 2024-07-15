using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Orders
{
    public class GetOrderByRefNoQuery
    {
        public GetOrderByRefNoQuery()
        {
            RefNo = null!;
            External = null!;
        }

        public string RefNo { get; set; }
        public string External { get; set; }

        public static GetOrderByRefNoQuery Create(string refNo, string external)
        {
            return new GetOrderByRefNoQuery
            {
                RefNo = refNo,
                External = external
            };
        }
    }
}