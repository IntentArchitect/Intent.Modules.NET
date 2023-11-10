using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFramework.SynchronousRepositories.Domain.Entities;
using EntityFramework.SynchronousRepositories.Domain.Repositories;
using EntityFramework.SynchronousRepositories.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFramework.SynchronousRepositories.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomerRepository : RepositoryBase<Customer, Customer, ApplicationDbContext>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<Customer?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<Customer>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }

        [IntentManaged(Mode.Fully)]
        public Customer? FindById(Guid id)
        {
            return Find(x => x.Id == id);
        }

        [IntentManaged(Mode.Fully)]
        public List<Customer> FindByIds(Guid[] ids)
        {
            return FindAll(x => ids.Contains(x.Id));
        }
    }
}