using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Customers
{
    public class GetCustomersPaginatedQuery
    {
        public GetCustomersPaginatedQuery()
        {
            Name = null!;
            Surname = null!;
        }

        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }

        public static GetCustomersPaginatedQuery Create(bool isActive, string name, string surname, int pageNo, int pageSize)
        {
            return new GetCustomersPaginatedQuery
            {
                IsActive = isActive,
                Name = name,
                Surname = surname,
                PageNo = pageNo,
                PageSize = pageSize
            };
        }
    }
}