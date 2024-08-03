using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.ValueObjects;
using EntityFrameworkCore.Postgres.Domain.Repositories;
using EntityFrameworkCore.Postgres.Domain.Repositories.ValueObjects;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Repositories.ValueObjects
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PersonWithAddressSerializedRepository : RepositoryBase<PersonWithAddressSerialized, PersonWithAddressSerialized, ApplicationDbContext>, IPersonWithAddressSerializedRepository
    {
        public PersonWithAddressSerializedRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<PersonWithAddressSerialized?> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<PersonWithAddressSerialized>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}