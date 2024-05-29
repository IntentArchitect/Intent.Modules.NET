using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.UniqueIndexConstraint;
using CleanArchitecture.Comprehensive.Domain.Repositories;
using CleanArchitecture.Comprehensive.Domain.Repositories.UniqueIndexConstraint;
using CleanArchitecture.Comprehensive.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Repositories.UniqueIndexConstraint
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AggregateWithUniqueConstraintIndexStereotypeRepository : RepositoryBase<AggregateWithUniqueConstraintIndexStereotype, AggregateWithUniqueConstraintIndexStereotype, ApplicationDbContext>, IAggregateWithUniqueConstraintIndexStereotypeRepository
    {
        public AggregateWithUniqueConstraintIndexStereotypeRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<AggregateWithUniqueConstraintIndexStereotype?> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<AggregateWithUniqueConstraintIndexStereotype>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}