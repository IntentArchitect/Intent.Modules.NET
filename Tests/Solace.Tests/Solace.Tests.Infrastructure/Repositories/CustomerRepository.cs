using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Solace.Tests.Application.Common.Models;
using Solace.Tests.Domain.Contracts;
using Solace.Tests.Domain.Entities;
using Solace.Tests.Domain.Repositories;
using Solace.Tests.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace Solace.Tests.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomerRepository : RepositoryBase<Customer, Customer, ApplicationDbContext>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<Customer>> SearchDapperAsync(CancellationToken cancellationToken = default)
        {
            var customers = await GetConnection().QueryAsync<Customer>("Select * from [dbo].[Customers]");
            return customers.ToList();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<Customer>> SearchSqlEFAsync(CancellationToken cancellationToken = default)
        {
            //return await _dbContext.Customers.FromSql($"Select c.* from [dbo].[Customers] c where IsActive = {true}").Include(c => c.Addresses).ToListAsync(cancellationToken);
            return await _dbContext.Customers.FromSql($"Select c.* from [dbo].[Customers] c").ToListAsync(cancellationToken);


        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<CustomerCustom>> SearchCustomResultAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Database.SqlQuery<CustomerCustom>($"Select * from [dbo].[Customers]").ToListAsync(cancellationToken);
        }

        public async Task<Customer?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<Customer>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}