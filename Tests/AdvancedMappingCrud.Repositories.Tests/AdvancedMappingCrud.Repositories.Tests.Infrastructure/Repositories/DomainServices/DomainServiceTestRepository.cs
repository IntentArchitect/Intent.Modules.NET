using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.DomainServices;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Repositories.DomainServices
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DomainServiceTestRepository : RepositoryBase<DomainServiceTest, DomainServiceTest, ApplicationDbContext>, IDomainServiceTestRepository
    {
        public DomainServiceTestRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<DomainServiceTest?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<DomainServiceTest>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}