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
    internal class EntityOfTRedisOmRepository<T> : RedisOmRepositoryBase<EntityOfT<T>, EntityOfTDocument<T>, IEntityOfTDocument<T>>, IEntityOfTRepository<T>
    {
        public EntityOfTRedisOmRepository(RedisOmUnitOfWork unitOfWork, RedisConnectionProvider connectionProvider) : base(unitOfWork, connectionProvider)
        {
        }

        public async Task<EntityOfT<T>?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);

        protected override string GetIdValue(EntityOfT<T> entity) => entity.Id;

        protected override void SetIdValue(EntityOfT<T> domainEntity, EntityOfTDocument<T> document) => ReflectionHelper.ForceSetProperty(domainEntity, "Id", document.Id);
    }
}