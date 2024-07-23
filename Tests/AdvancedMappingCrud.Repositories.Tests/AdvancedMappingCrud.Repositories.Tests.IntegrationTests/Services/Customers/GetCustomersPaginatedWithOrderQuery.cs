using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Customers
{
    public class GetCustomersPaginatedWithOrderQuery
    {
        public GetCustomersPaginatedWithOrderQuery()
        {
            Name = null!;
            Surname = null!;
            OrderBy = null!;
        }

        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }

        public static GetCustomersPaginatedWithOrderQuery Create(
            bool isActive,
            string name,
            string surname,
            int pageNo,
            int pageSize,
            string orderBy)
        {
            return new GetCustomersPaginatedWithOrderQuery
            {
                IsActive = isActive,
                Name = name,
                Surname = surname,
                PageNo = pageNo,
                PageSize = pageSize,
                OrderBy = orderBy
            };
        }
    }
}