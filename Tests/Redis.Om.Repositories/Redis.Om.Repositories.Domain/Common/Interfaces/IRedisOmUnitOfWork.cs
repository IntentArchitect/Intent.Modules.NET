using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmUnitOfWorkInterface", Version = "1.0")]

namespace Redis.Om.Repositories.Domain.Common.Interfaces
{
    public interface IRedisOmUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}