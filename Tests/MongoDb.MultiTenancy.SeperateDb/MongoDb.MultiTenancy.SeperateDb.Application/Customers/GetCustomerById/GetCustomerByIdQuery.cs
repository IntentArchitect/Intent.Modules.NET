using Intent.RoslynWeaver.Attributes;
using MediatR;
using MongoDb.MultiTenancy.SeperateDb.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace MongoDb.MultiTenancy.SeperateDb.Application.Customers.GetCustomerById
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