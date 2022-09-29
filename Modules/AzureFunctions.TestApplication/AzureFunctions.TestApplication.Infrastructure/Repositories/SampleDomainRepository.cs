using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Domain;
using AzureFunctions.TestApplication.Domain.Repositories;
using AzureFunctions.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace AzureFunctions.TestApplication.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SampleDomainRepository : RepositoryBase<ISampleDomain, SampleDomain, ApplicationDbContext>, ISampleDomainRepository
    {
        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public SampleDomainRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }


        [IntentManaged(Mode.Fully)]
        public async Task<ISampleDomain> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }


        [IntentManaged(Mode.Fully)]
        public async Task<List<ISampleDomain>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}