using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Entities;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Repositories;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class BasketRepository : RepositoryBase<Basket, Basket, ApplicationDbContext>, IBasketRepository
    {
        public BasketRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<Basket?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<Basket>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}