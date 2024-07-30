using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.Products
{
    public class GetProductsPaginatedWithOrderQuery
    {
        public GetProductsPaginatedWithOrderQuery()
        {
            OrderBy = null!;
        }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }

        public static GetProductsPaginatedWithOrderQuery Create(int pageNo, int pageSize, string orderBy)
        {
            return new GetProductsPaginatedWithOrderQuery
            {
                PageNo = pageNo,
                PageSize = pageSize,
                OrderBy = orderBy
            };
        }
    }
}