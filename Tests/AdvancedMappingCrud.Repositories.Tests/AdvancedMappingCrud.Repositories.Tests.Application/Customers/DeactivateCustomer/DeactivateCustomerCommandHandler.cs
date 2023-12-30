using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Services;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.DeactivateCustomer
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DeactivateCustomerCommandHandler : IRequestHandler<DeactivateCustomerCommand>
    {
        private readonly ICustomerManager _customerManager;

        [IntentManaged(Mode.Fully)]
        public DeactivateCustomerCommandHandler(ICustomerManager customerManager)
        {
            _customerManager = customerManager;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeactivateCustomerCommand request, CancellationToken cancellationToken)
        {
            await _customerManager.DeactivateCustomerAsync(request.CustomerId, cancellationToken);
        }
    }
}