using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Services
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomerManager : ICustomerManager
    {
        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public CustomerManager()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public CustomerStatistics GetCustomerStatistics(Guid customerId)
        {
            // TODO: Implement GetCustomerStatistics (CustomerManager) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task DeactivateCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeactivateCustomerAsync (CustomerManager) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}