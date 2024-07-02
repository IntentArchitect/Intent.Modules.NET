using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Caching.Distributed;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.DistributedCaching.IDistributedCacheExtensions", Version = "1.0")]

namespace EfCore.SecondLevelCaching.Application.Common.Interfaces
{
    public static class DistributedCacheExtensions
    {
        public static T? Get<T>(this IDistributedCache cache, string key)
        {
            var bytes = cache.Get(key);

            if (bytes == null)
            {
                return default;
            }
            var json = Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<T>(json);
        }

        public static async Task<T?> GetAsync<T>(
            this IDistributedCache cache,
            string key,
            CancellationToken cancellationToken = default)
        {
            var bytes = await cache.GetAsync(key, cancellationToken);

            if (bytes == null)
            {
                return default;
            }
            var json = Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<T>(json);
        }

        public static void Set<T>(
            this IDistributedCache cache,
            string key,
            T value,
            DistributedCacheEntryOptions? options = default)
        {
            var json = JsonSerializer.Serialize(value);
            var bytes = Encoding.UTF8.GetBytes(json);
            cache.Set(key, bytes, options ?? new DistributedCacheEntryOptions());
        }

        public static async Task SetAsync<T>(
            this IDistributedCache cache,
            string key,
            T value,
            DistributedCacheEntryOptions? options = default,
            CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(value);
            var bytes = Encoding.UTF8.GetBytes(json);
            await cache.SetAsync(key, bytes, options ?? new DistributedCacheEntryOptions(), cancellationToken);
        }
    }
}