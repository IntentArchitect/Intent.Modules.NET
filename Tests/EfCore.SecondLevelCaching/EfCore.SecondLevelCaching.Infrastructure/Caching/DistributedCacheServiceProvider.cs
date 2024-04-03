using System;
using System.Collections.Generic;
using EfCore.SecondLevelCaching.Application.Common.Interfaces;
using EFCoreSecondLevelCacheInterceptor;
using Intent.RoslynWeaver.Attributes;
using MessagePack;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.SecondLevelCaching.DistributedCacheServiceProvider", Version = "1.0")]

namespace EfCore.SecondLevelCaching.Infrastructure.Caching
{
    public class DistributedCacheServiceProvider : IEFCacheServiceProvider
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IEFDebugLogger _efDebugLogger;
        private readonly ILogger<DistributedCacheServiceProvider> _logger;

        public DistributedCacheServiceProvider(IDistributedCacheWithUnitOfWork distributedCache,
            IEFDebugLogger efDebugLogger,
            ILogger<DistributedCacheServiceProvider> logger)
        {
            _distributedCache = distributedCache;
            _efDebugLogger = efDebugLogger;
            _logger = logger;
        }

        public void InsertValue(EFCacheKey cacheKey, EFCachedData? value, EFCachePolicy cachePolicy)
        {
            if (cacheKey is null)
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }

            if (cachePolicy is null)
            {
                throw new ArgumentNullException(nameof(cachePolicy));
            }
            value ??= new EFCachedData { IsNull = true };

            var keyHash = cacheKey.KeyHash;

            foreach (var rootCacheKey in cacheKey.CacheDependencies)
            {
                if (string.IsNullOrWhiteSpace(rootCacheKey))
                {
                    continue;
                }
                var items = GetCacheItem<HashSet<string>>(rootCacheKey);
                if (items is null)
                {
                    items = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { keyHash };
                    SetCacheItem(rootCacheKey, items, cachePolicy);
                }
                else
                {
                    items.Add(keyHash);
                    SetCacheItem(rootCacheKey, items, cachePolicy);
                }
            }
            SetCacheItem(keyHash, value, cachePolicy);
        }

        public void ClearAllCachedEntries()
        {
            // NOP, unsupported
        }

        public EFCachedData? GetValue(EFCacheKey cacheKey, EFCachePolicy cachePolicy)
        {
            if (cacheKey is null)
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }
            return GetCacheItem<EFCachedData>(cacheKey.KeyHash);
        }

        public void InvalidateCacheDependencies(EFCacheKey cacheKey)
        {
            if (cacheKey is null)
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }

            foreach (var rootCacheKey in cacheKey.CacheDependencies)
            {
                if (string.IsNullOrWhiteSpace(rootCacheKey))
                {
                    continue;
                }
                var cachedValue = GetCacheItem<EFCachedData>(cacheKey.KeyHash);
                var dependencyKeys = GetCacheItem<HashSet<string>>(rootCacheKey);
                if (AreRootCacheKeysExpired(cachedValue, dependencyKeys))
                {
                    if (_efDebugLogger.IsLoggerEnabled)
                    {
                        _logger.LogDebug(CacheableEventId.QueryResultInvalidated, "Invalidated all of the cache entries due to early expiration of a root cache key[{RootCacheKey}].", rootCacheKey);
                    }
                    ClearAllCachedEntries();
                    return;
                }
                ClearDependencyValues(dependencyKeys);
                _distributedCache.Remove(rootCacheKey);
            }
        }

        private void ClearDependencyValues(HashSet<string>? dependencyKeys)
        {
            if (dependencyKeys is null)
            {
                return;
            }

            foreach (var dependencyKey in dependencyKeys)
            {
                _distributedCache.Remove(dependencyKey);
            }
        }

        private static bool AreRootCacheKeysExpired(EFCachedData? cachedValue, HashSet<string>? dependencyKeys) => cachedValue is not null && dependencyKeys is null;

        private T? GetCacheItem<T>(string key)
            where T : class
        {
            var serialized = _distributedCache.Get(key);

            if (serialized == null)
            {
                return null;
            }
            return (T)MessagePackSerializer.Typeless.Deserialize(serialized)!;
        }

        private void SetCacheItem<T>(string key, T value, EFCachePolicy cachePolicy)
        {
            var cacheEntryOptions = cachePolicy.CacheExpirationMode switch
            {
                CacheExpirationMode.Absolute => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = cachePolicy.CacheTimeout
                },
                CacheExpirationMode.Sliding => new DistributedCacheEntryOptions
                {
                    SlidingExpiration = cachePolicy.CacheTimeout
                },
                _ => throw new ArgumentOutOfRangeException()
            };
            var serialized = MessagePackSerializer.Typeless.Serialize(value);
            _distributedCache.Set(key, serialized, cacheEntryOptions);
        }
    }
}