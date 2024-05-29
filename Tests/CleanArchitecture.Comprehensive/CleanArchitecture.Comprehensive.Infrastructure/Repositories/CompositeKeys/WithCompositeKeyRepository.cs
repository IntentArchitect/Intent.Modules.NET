using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.CompositeKeys;
using CleanArchitecture.Comprehensive.Domain.Repositories;
using CleanArchitecture.Comprehensive.Domain.Repositories.CompositeKeys;
using CleanArchitecture.Comprehensive.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Repositories.CompositeKeys
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class WithCompositeKeyRepository : RepositoryBase<WithCompositeKey, WithCompositeKey, ApplicationDbContext>, IWithCompositeKeyRepository
    {
        public WithCompositeKeyRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<WithCompositeKey?> FindByIdAsync(
            (Guid Key1Id, Guid Key2Id) id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Key1Id == id.Key1Id && x.Key2Id == id.Key2Id, cancellationToken);
        }
    }
}