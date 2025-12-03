using Intent.Modules.NET.Tests.Application.Core.Common.Eventing;
using Intent.Modules.NET.Tests.Module2.Domain.Entities;
using Intent.Modules.NET.Tests.Module2.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using Module1.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module2.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomerCreatedIEHandler : IIntegrationEventHandler<CustomerCreatedIEEvent>
    {
        private IMyCustomerRepository _repo;

        [IntentManaged(Mode.Merge)]
        public CustomerCreatedIEHandler(IMyCustomerRepository repo)
        {
            _repo = repo;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(CustomerCreatedIEEvent message, CancellationToken cancellationToken = default)
        {
            MyCustomer customer = new MyCustomer() { Id = message.Customer.Id, Name = message.Customer.Name };
            _repo.Add(customer);
            await _repo.UnitOfWork.SaveChangesAsync();
        }
    }
}