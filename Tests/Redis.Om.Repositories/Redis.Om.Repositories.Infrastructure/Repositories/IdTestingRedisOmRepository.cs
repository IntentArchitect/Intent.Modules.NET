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
    internal class IdTestingRedisOmRepository : RedisOmRepositoryBase<IdTesting, IdTestingDocument, IIdTestingDocument>, IIdTestingRepository
    {
        public IdTestingRedisOmRepository(RedisOmUnitOfWork unitOfWork, RedisConnectionProvider connectionProvider) : base(unitOfWork, connectionProvider)
        {
        }

        public async Task<IdTesting?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);

        protected override string GetIdValue(IdTesting entity) => entity.Identifier;

        protected override void SetIdValue(IdTesting domainEntity, IdTestingDocument document) => ReflectionHelper.ForceSetProperty(domainEntity, "Identifier", document.Identifier);
    }
}