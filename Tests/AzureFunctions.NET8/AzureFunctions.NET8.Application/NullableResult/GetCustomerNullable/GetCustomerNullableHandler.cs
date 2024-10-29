using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET8.Application.Customers;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AzureFunctions.NET8.Application.NullableResult.GetCustomerNullable
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerNullableHandler : IRequestHandler<GetCustomerNullable, CustomerDto>
    {
        [IntentManaged(Mode.Ignore)]
        public GetCustomerNullableHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<CustomerDto> Handle(GetCustomerNullable request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}