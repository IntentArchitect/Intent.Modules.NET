using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.Products
{
    public class GetProductsPaginatedQuery
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }

        public static GetProductsPaginatedQuery Create(int pageNo, int pageSize)
        {
            return new GetProductsPaginatedQuery
            {
                PageNo = pageNo,
                PageSize = pageSize
            };
        }
    }
}