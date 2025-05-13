using AutoMapper;
using Intent.Modules.NET.Tests.Module2.Domain.Entities;
using Intent.Modules.NET.Tests.Module2.Domain.Repositories;
using Intent.Modules.NET.Tests.Module2.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module2.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MyCustomerRepository : RepositoryBase<MyCustomer, MyCustomer, Module2DbContext>, IMyCustomerRepository
    {
        public MyCustomerRepository(Module2DbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<MyCustomer?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<MyCustomer>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}