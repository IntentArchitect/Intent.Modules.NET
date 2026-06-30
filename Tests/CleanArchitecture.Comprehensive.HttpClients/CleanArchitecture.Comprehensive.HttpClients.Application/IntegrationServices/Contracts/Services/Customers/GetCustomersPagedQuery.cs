using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers
{
    public class GetCustomersPagedQuery
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }

        public static GetCustomersPagedQuery Create(int pageNo, int pageSize)
        {
            return new GetCustomersPagedQuery
            {
                PageNo = pageNo,
                PageSize = pageSize
            };
        }
    }
}