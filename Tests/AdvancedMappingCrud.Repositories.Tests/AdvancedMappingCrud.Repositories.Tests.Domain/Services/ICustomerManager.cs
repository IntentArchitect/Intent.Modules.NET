using System;
using AdvancedMappingCrud.Repositories.Tests.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Services
{
    public interface ICustomerManager
    {
        CustomerStatistics GetCustomerStatistics(Guid customerId);
        void DeactivateCustomer(Guid customerId);
    }
}