using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Domain.Common.Interfaces;
using Ardalis.Domain.Entities;
using Ardalis.Specification;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Ardalis.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IClientRepository : IRepositoryBase<Client>, IClientReadRepository
    {
        IUnitOfWork UnitOfWork { get; }
        void Add(Client entity);
        void Remove(Client entity);
    }
}