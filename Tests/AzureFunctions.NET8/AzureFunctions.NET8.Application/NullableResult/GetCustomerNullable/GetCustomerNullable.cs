using AzureFunctions.NET6.Application.Common.Interfaces;
using AzureFunctions.NET6.Application.Customers;
using AzureFunctions.NET8.Application.Common.Interfaces;
using AzureFunctions.NET8.Application.Customers;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AzureFunctions.NET8.Application.NullableResult.GetCustomerNullable
{
    public class GetCustomerNullable : IRequest<CustomerDto>, IQuery
    {
        public GetCustomerNullable()
        {
        }
    }
}