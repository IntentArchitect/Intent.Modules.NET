using System;
using AzureFunctions.NET6.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AzureFunctions.NET6.Application.Customers.GetCustomerThing
{
    /// <summary>
    /// Check CustomerId route parameter generates correctly
    /// </summary>
    public class GetCustomerThing : IRequest<CustomerDto>, IQuery
    {
        public GetCustomerThing(Guid customerId)
        {
            CustomerId = customerId;
        }

        public Guid CustomerId { get; set; }
    }
}