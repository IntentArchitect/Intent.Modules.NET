using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.Application.Customers.GetCustomerById
{
    public class GetCustomerByIdQuery : IRequest<CustomerDto>, IQuery
    {
        public GetCustomerByIdQuery(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}