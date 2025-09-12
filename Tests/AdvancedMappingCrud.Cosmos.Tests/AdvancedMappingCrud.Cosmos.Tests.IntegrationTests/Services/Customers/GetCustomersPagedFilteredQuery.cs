using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Customers
{
    public class GetCustomersPagedFilteredQuery
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? OrderBy { get; set; }
        public bool IsActive { get; set; }

        public static GetCustomersPagedFilteredQuery Create(int pageNo, int pageSize, string? orderBy, bool isActive)
        {
            return new GetCustomersPagedFilteredQuery
            {
                PageNo = pageNo,
                PageSize = pageSize,
                OrderBy = orderBy,
                IsActive = isActive
            };
        }
    }
}