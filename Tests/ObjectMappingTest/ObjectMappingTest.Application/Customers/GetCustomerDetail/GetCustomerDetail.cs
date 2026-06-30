using Intent.RoslynWeaver.Attributes;
using MediatR;
using ObjectMappingTest.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace ObjectMappingTest.Application.Customers.GetCustomerDetail
{
    public class GetCustomerDetail : IRequest<CustomerDetailDto>, IQuery
    {
        public GetCustomerDetail(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}