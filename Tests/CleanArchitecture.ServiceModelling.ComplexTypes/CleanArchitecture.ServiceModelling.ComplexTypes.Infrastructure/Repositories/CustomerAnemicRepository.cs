using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Entities;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Repositories;
using CleanArchitecture.ServiceModelling.ComplexTypes.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomerAnemicRepository : RepositoryBase<CustomerAnemic, CustomerAnemic, ApplicationDbContext>, ICustomerAnemicRepository
    {
        public CustomerAnemicRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<CustomerAnemic?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<CustomerAnemic>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}