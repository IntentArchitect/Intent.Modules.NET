using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Interfaces.Customers
{
    public interface IPersonService : IDisposable
    {
        Task<PersonDto> GetPersonById(Guid personId, CancellationToken cancellationToken = default);
    }
}