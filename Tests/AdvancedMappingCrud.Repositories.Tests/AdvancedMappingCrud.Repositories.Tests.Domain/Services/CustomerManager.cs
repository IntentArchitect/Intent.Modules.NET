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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CustomerStatistics GetCustomerStatistics(Guid customerId)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task DeactivateCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}