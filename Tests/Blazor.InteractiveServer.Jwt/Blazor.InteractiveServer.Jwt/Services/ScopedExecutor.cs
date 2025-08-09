using System;
using System.Runtime.CompilerServices;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.ScopedExecutorTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.Jwt.Services
{
    public class ScopedExecutor : IScopedExecutor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IServiceProvider _serviceProvider;

        public ScopedExecutor(IServiceScopeFactory scopeFactory, IServiceProvider serviceProvider)
        {
            _scopeFactory = scopeFactory;
            _serviceProvider = serviceProvider;
        }

        public async Task<IServiceScope> CreateScope()
        {
            return _scopeFactory.CreateScope();
        }

        public async Task ExecuteAsync(Func<IServiceProvider, Task> action)
        {
            using var scope = await CreateScope();
            await action(scope.ServiceProvider);
        }

        public async Task<T> ExecuteAsync<T>(Func<IServiceProvider, Task<T>> action)
        {
            using var scope = await CreateScope();
            return await action(scope.ServiceProvider);
        }

        public async IAsyncEnumerable<T> ExecuteStreamAsync<T>(
            Func<IServiceProvider, IAsyncEnumerable<T>> action,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var scope = await CreateScope();
            var stream = action(scope.ServiceProvider);
            await foreach (var item in stream.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                yield return item;
            }
        }
    }
}