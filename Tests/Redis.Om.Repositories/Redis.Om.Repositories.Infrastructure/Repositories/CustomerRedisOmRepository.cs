using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Redis.OM;
using Redis.Om.Repositories.Domain.Entities;
using Redis.Om.Repositories.Domain.Repositories;
using Redis.Om.Repositories.Domain.Repositories.Documents;
using Redis.Om.Repositories.Infrastructure.Persistence;
using Redis.Om.Repositories.Infrastructure.Persistence.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmRepository", Version = "1.0")]

namespace Redis.Om.Repositories.Infrastructure.Repositories
{
    internal class CustomerRedisOmRepository : RedisOmRepositoryBase<Customer, CustomerDocument, ICustomerDocument>, ICustomerRepository
    {
        public CustomerRedisOmRepository(RedisOmUnitOfWork unitOfWork, RedisConnectionProvider connectionProvider) : base(unitOfWork, connectionProvider)
        {
        }

        public async Task<Customer?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);

        protected override string GetIdValue(Customer entity) => entity.Id;

        protected override void SetIdValue(Customer domainEntity, CustomerDocument document) => ReflectionHelper.ForceSetProperty(domainEntity, "Id", document.Id);
    }
}