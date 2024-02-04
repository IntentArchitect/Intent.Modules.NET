using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Domain.Entities;
using MassTransit.RabbitMQ.Domain.Repositories;
using MassTransit.RabbitMQ.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace MassTransit.RabbitMQ.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AnimalRepository : RepositoryBase<Animal, Animal, ApplicationDbContext>, IAnimalRepository
    {
        public AnimalRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<Animal?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<Animal>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}