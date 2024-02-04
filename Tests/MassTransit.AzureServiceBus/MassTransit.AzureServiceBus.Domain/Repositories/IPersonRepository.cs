using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPersonRepository : IEFRepository<Person, Person>
    {
        [IntentManaged(Mode.Fully)]
        Task<Person?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Person>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}