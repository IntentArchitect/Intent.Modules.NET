using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.Products
{
    public class GetProductsPaginatedByNameQuery
    {
        public GetProductsPaginatedByNameQuery()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }

        public static GetProductsPaginatedByNameQuery Create(string name, int pageNo, int pageSize)
        {
            return new GetProductsPaginatedByNameQuery
            {
                Name = name,
                PageNo = pageNo,
                PageSize = pageSize
            };
        }
    }
}