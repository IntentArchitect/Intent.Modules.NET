#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace EfCore.SecondLevelCaching.Application.UnitTests.Caching
{
    /// <summary>
    /// An <see cref="IDistributedCache"/> that records every write, including the
    /// <see cref="DistributedCacheEntryOptions"/> it was given, so tests can assert on what reached the cache.
    /// </summary>
    public sealed class RecordingDistributedCache : IDistributedCache
    {
        public sealed record SetCall(string Method, string Key, byte[] Value, DistributedCacheEntryOptions Options);

        public List<SetCall> SetCalls { get; } = new();

        public byte[]? Get(string key) => null;

        public Task<byte[]?> GetAsync(string key, CancellationToken token = default) => Task.FromResult<byte[]?>(null);

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
            => SetCalls.Add(new SetCall(nameof(Set), key, value, options));

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            SetCalls.Add(new SetCall(nameof(SetAsync), key, value, options));
            return Task.CompletedTask;
        }

        public void Refresh(string key)
        {
        }

        public Task RefreshAsync(string key, CancellationToken token = default) => Task.CompletedTask;

        public void Remove(string key)
        {
        }

        public Task RemoveAsync(string key, CancellationToken token = default) => Task.CompletedTask;
    }
}
