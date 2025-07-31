using EfCoreSoftDelete.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace EfCoreSoftDelete.Application.Customers.GetDeletedCustomerById
{
    public class GetDeletedCustomerByIdQuery : IRequest<CustomerDto>, IQuery
    {
        public GetDeletedCustomerByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}