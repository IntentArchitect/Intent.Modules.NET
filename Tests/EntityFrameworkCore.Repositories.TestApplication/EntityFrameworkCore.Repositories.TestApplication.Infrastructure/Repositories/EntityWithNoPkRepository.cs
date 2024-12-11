using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class EntityWithNoPkRepository : RepositoryBase<EntityWithNoPk, EntityWithNoPk, ApplicationDbContext>, IEntityWithNoPkRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EntityWithNoPkRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
        }

        public async Task StoredProcedure(CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE StoredProcedure", cancellationToken);
        }

        public void Add(EntityWithNoPk entity)
        {
            _dbContext.Database.ExecuteSqlInterpolated($"INSERT INTO EntityWithNoPks (Name) VALUES({entity.Name})");
        }
    }
}