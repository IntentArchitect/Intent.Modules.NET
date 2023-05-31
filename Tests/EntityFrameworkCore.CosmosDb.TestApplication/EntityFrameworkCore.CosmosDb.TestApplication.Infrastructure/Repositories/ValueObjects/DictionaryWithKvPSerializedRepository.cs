using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.ValueObjects;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.ValueObjects;
using EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Repositories.ValueObjects
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DictionaryWithKvPSerializedRepository : RepositoryBase<DictionaryWithKvPSerialized, DictionaryWithKvPSerialized, ApplicationDbContext>, IDictionaryWithKvPSerializedRepository
    {
        public DictionaryWithKvPSerializedRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<DictionaryWithKvPSerialized> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken) ?? throw new Exception("Id not found.");
        }

        public async Task<List<DictionaryWithKvPSerialized>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}