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
    internal class BaseTypeRedisOmRepository : RedisOmRepositoryBase<BaseType, BaseTypeDocument, IBaseTypeDocument>, IBaseTypeRepository
    {
        public BaseTypeRedisOmRepository(RedisOmUnitOfWork unitOfWork, RedisConnectionProvider connectionProvider) : base(unitOfWork, connectionProvider)
        {
        }

        public async Task<BaseType?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);

        protected override string GetIdValue(BaseType entity) => entity.Id;

        protected override void SetIdValue(BaseType domainEntity, BaseTypeDocument document) => ReflectionHelper.ForceSetProperty(domainEntity, "Id", document.Id);
    }
}