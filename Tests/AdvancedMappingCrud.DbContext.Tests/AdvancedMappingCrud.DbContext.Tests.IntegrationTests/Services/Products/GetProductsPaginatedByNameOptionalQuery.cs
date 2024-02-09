using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.Products
{
    public class GetProductsPaginatedByNameOptionalQuery
    {
        public string? Name { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }

        public static GetProductsPaginatedByNameOptionalQuery Create(string? name, int pageNo, int pageSize)
        {
            return new GetProductsPaginatedByNameOptionalQuery
            {
                Name = name,
                PageNo = pageNo,
                PageSize = pageSize
            };
        }
    }
}