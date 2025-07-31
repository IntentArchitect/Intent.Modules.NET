using EfCoreSoftDelete.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace EfCoreSoftDelete.Application.Customers.GetCustomers
{
    public class GetCustomersQuery : IRequest<List<CustomerDto>>, IQuery
    {
        public GetCustomersQuery()
        {
        }
    }
}