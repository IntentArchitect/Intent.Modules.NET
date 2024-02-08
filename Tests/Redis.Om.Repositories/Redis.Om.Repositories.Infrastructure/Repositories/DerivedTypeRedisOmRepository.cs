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
    internal class DerivedTypeRedisOmRepository : RedisOmRepositoryBase<DerivedType, DerivedTypeDocument, IDerivedTypeDocument>, IDerivedTypeRepository
    {
        public DerivedTypeRedisOmRepository(RedisOmUnitOfWork unitOfWork, RedisConnectionProvider connectionProvider) : base(unitOfWork, connectionProvider)
        {
        }

        public async Task<DerivedType?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);

        protected override string GetIdValue(DerivedType entity) => entity.Id;

        protected override void SetIdValue(DerivedType domainEntity, DerivedTypeDocument document) => ReflectionHelper.ForceSetProperty(domainEntity, "Id", document.Id);
    }
}