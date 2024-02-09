using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Orders
{
    public class GetOrdersPaginatedQuery
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }

        public static GetOrdersPaginatedQuery Create(int pageNo, int pageSize)
        {
            return new GetOrdersPaginatedQuery
            {
                PageNo = pageNo,
                PageSize = pageSize
            };
        }
    }
}