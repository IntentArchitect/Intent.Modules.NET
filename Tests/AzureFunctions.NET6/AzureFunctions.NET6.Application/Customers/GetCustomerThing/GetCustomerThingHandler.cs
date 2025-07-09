using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AzureFunctions.NET6.Application.Customers.GetCustomerThing
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerThingHandler : IRequestHandler<GetCustomerThing, CustomerDto>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomerThingHandler()
        {
        }

        /// <summary>
        /// Check CustomerId route parameter generates correctly
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<CustomerDto> Handle(GetCustomerThing request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetCustomerThingHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}