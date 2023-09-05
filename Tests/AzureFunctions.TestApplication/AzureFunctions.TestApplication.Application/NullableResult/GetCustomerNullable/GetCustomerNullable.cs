using AzureFunctions.TestApplication.Application.Common.Interfaces;
using AzureFunctions.TestApplication.Application.Customers;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.NullableResult.GetCustomerNullable
{
    public class GetCustomerNullable : IRequest<CustomerDto>, IQuery
    {
        public GetCustomerNullable()
        {
        }
    }
}