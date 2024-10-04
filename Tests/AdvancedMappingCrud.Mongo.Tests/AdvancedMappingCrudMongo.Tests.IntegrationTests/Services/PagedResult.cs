using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.PagedResult", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services
{
    public class PagedResult<TData>
    {
        public PagedResult()
        {
            Data = null!;
        }

        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public List<TData> Data { get; set; }
    }
}