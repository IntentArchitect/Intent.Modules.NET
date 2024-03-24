using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Caching.Distributed;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.DistributedCaching.DistributedCacheWithUnitOfWorkInterface", Version = "1.0")]

namespace EfCore.SecondLevelCaching.Application.Common.Interfaces
{
    public interface IDistributedCacheWithUnitOfWork : IDistributedCache
    {
        IDisposable EnableUnitOfWork();
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}