using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.ODataQuery;
using CleanArchitecture.Comprehensive.Domain.Repositories;
using CleanArchitecture.Comprehensive.Domain.Repositories.ODataQuery;
using CleanArchitecture.Comprehensive.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Repositories.ODataQuery
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ODataAggRepository : RepositoryBase<ODataAgg, ODataAgg, ApplicationDbContext>, IODataAggRepository
    {
        public ODataAggRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<ODataAgg?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<ODataAgg>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}