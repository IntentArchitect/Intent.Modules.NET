using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET6.Application.Customers;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AzureFunctions.NET6.Application.NullableResult.GetCustomerNullable
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerNullableHandler : IRequestHandler<GetCustomerNullable, CustomerDto>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomerNullableHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<CustomerDto> Handle(GetCustomerNullable request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetCustomerNullableHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}