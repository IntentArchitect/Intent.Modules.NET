using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Domain.Entities;
using Ardalis.Domain.Repositories;
using Ardalis.Infrastructure.Persistence;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace Ardalis.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ClientRepository : RepositoryBase<Client, ApplicationDbContext>, IClientRepository
    {
        public ClientRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        [IntentManaged(Mode.Fully)]
        public Task<Client?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return GetByIdAsync(id: id, cancellationToken: cancellationToken);
        }
    }
}