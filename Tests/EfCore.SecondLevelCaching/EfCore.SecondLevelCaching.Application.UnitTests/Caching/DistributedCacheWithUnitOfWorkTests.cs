#nullable enable
using System;
using System.Threading.Tasks;
using EfCore.SecondLevelCaching.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Xunit;

namespace EfCore.SecondLevelCaching.Application.UnitTests.Caching
{
    /// <summary>
    /// Regression tests for the generated <see cref="DistributedCacheWithUnitOfWork"/>: the
    /// <see cref="DistributedCacheEntryOptions"/> a caller passes to Set/SetAsync must reach the underlying
    /// cache whether the write happens immediately or is deferred until the unit of work commits.
    /// </summary>
    public class DistributedCacheWithUnitOfWorkTests
    {
        private static readonly TimeSpan Expiry = TimeSpan.FromMinutes(5);

        private static DistributedCacheEntryOptions CreateOptions() => new() { AbsoluteExpirationRelativeToNow = Expiry };

        [Fact]
        public async Task SetAsync_PassesOptionsThrough_WhenNoUnitOfWork()
        {
            // Arrange
            var cache = new RecordingDistributedCache();
            var sut = new DistributedCacheWithUnitOfWork(cache);

            // Act
            await sut.SetAsync("key", new byte[] { 1 }, CreateOptions(), TestContext.Current.CancellationToken);

            // Assert
            var call = Assert.Single(cache.SetCalls);
            Assert.Equal(Expiry, call.Options.AbsoluteExpirationRelativeToNow);
        }

        [Fact]
        public async Task SetAsync_PassesOptionsOnCommit_WhenInsideUnitOfWork()
        {
            // Arrange
            var cache = new RecordingDistributedCache();
            var sut = new DistributedCacheWithUnitOfWork(cache);

            // Act
            using (sut.EnableUnitOfWork())
            {
                await sut.SetAsync("key", new byte[] { 1 }, CreateOptions(), TestContext.Current.CancellationToken);
                Assert.Empty(cache.SetCalls); // deferred until commit
                await sut.SaveChangesAsync(TestContext.Current.CancellationToken);
            }

            // Assert
            var call = Assert.Single(cache.SetCalls);
            Assert.Equal(Expiry, call.Options.AbsoluteExpirationRelativeToNow);
        }

        [Fact]
        public async Task Set_PassesOptionsOnCommit_WhenInsideUnitOfWork()
        {
            // Arrange
            var cache = new RecordingDistributedCache();
            var sut = new DistributedCacheWithUnitOfWork(cache);

            // Act
            using (sut.EnableUnitOfWork())
            {
                sut.Set("key", new byte[] { 1 }, CreateOptions());
                Assert.Empty(cache.SetCalls); // deferred until commit
                await sut.SaveChangesAsync(TestContext.Current.CancellationToken);
            }

            // Assert
            var call = Assert.Single(cache.SetCalls);
            Assert.Equal(Expiry, call.Options.AbsoluteExpirationRelativeToNow);
        }
    }
}
