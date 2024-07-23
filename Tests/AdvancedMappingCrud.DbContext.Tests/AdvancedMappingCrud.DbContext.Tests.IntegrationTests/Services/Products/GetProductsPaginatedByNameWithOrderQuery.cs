using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.Products
{
    public class GetProductsPaginatedByNameWithOrderQuery
    {
        public GetProductsPaginatedByNameWithOrderQuery()
        {
            Name = null!;
            OrderBy = null!;
        }

        public string Name { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }

        public static GetProductsPaginatedByNameWithOrderQuery Create(string name, int pageNo, int pageSize, string orderBy)
        {
            return new GetProductsPaginatedByNameWithOrderQuery
            {
                Name = name,
                PageNo = pageNo,
                PageSize = pageSize,
                OrderBy = orderBy
            };
        }
    }
}