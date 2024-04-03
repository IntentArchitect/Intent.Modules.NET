using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EfCore.SecondLevelCaching.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Caching.Distributed;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.DistributedCaching.DistributedCacheWithUnitOfWork", Version = "1.0")]

namespace EfCore.SecondLevelCaching.Infrastructure.Caching
{
    public class DistributedCacheWithUnitOfWork : IDistributedCacheWithUnitOfWork
    {
        private readonly AsyncLocal<ScopedData?> _scopedData = new AsyncLocal<ScopedData?>();
        private readonly IDistributedCache _distributedCache;

        public DistributedCacheWithUnitOfWork(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public IDisposable EnableUnitOfWork()
        {
            _scopedData.Value ??= new ScopedData(() => _scopedData.Value = null);

            return _scopedData.Value;
        }

        public byte[]? Get(string key)
        {
            if (_scopedData.Value == null)
            {
                return _distributedCache.Get(key);
            }

            if (!_scopedData.Value.Cache.TryGetValue(key, out var value))
            {
                return _distributedCache.Get(key);
            }
            return value;
        }

        public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            if (_scopedData.Value == null)
            {
                return await _distributedCache.GetAsync(key, token);
            }

            if (!_scopedData.Value.Cache.TryGetValue(key, out var value))
            {
                return await _distributedCache.GetAsync(key, token);
            }
            return value;
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            if (_scopedData.Value == null)
            {
                _distributedCache.Set(key, value, options);
                return;
            }
            _scopedData.Value.ActionQueue.Enqueue(cancellationToken => _distributedCache.SetAsync(key, value, cancellationToken));
            _scopedData.Value.Cache.AddOrUpdate(key, value, (_, _) => value);
        }

        public async Task SetAsync(
            string key,
            byte[] value,
            DistributedCacheEntryOptions options,
            CancellationToken token = default)
        {
            if (_scopedData.Value == null)
            {
                await _distributedCache.SetAsync(key, value, options, token);
                return;
            }
            _scopedData.Value.ActionQueue.Enqueue(cancellationToken => _distributedCache.SetAsync(key, value, cancellationToken));
            _scopedData.Value.Cache.AddOrUpdate(key, value, (_, _) => value);
        }

        public void Refresh(string key)
        {
            if (_scopedData.Value == null)
            {
                _distributedCache.Refresh(key);
                return;
            }
            _scopedData.Value.ActionQueue.Enqueue(cancellationToken => _distributedCache.RefreshAsync(key, cancellationToken));
        }

        public async Task RefreshAsync(string key, CancellationToken token = default)
        {
            if (_scopedData.Value == null)
            {
                await _distributedCache.RefreshAsync(key, token);
                return;
            }
            _scopedData.Value.ActionQueue.Enqueue(cancellationToken => _distributedCache.RefreshAsync(key, cancellationToken));
        }

        public void Remove(string key)
        {
            if (_scopedData.Value == null)
            {
                _distributedCache.Remove(key);
                return;
            }
            _scopedData.Value.ActionQueue.Enqueue(cancellationToken => _distributedCache.RemoveAsync(key, cancellationToken));
            _scopedData.Value.Cache.AddOrUpdate(key, (byte[]?)null, (_, _) => null);
        }

        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            if (_scopedData.Value == null)
            {
                await _distributedCache.RemoveAsync(key, token);
                return;
            }
            _scopedData.Value.ActionQueue.Enqueue(cancellationToken => _distributedCache.RemoveAsync(key, cancellationToken));
            _scopedData.Value.Cache.AddOrUpdate(key, (byte[]?)null, (_, _) => null);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested &&
                _scopedData.Value != null &&
                _scopedData.Value.ActionQueue.TryDequeue(out var action))
            {
                await action(cancellationToken);
            }
        }

        private class ScopedData : IDisposable
        {
            private readonly Action _disposeAction;

            public ScopedData(Action disposeAction)
            {
                _disposeAction = disposeAction;
            }

            public Queue<Func<CancellationToken, Task>> ActionQueue { get; } = new Queue<Func<CancellationToken, Task>>();
            public ConcurrentDictionary<string, byte[]?> Cache { get; } = new ConcurrentDictionary<string, byte[]?>();

            public void Dispose() => _disposeAction();
        }
    }
}