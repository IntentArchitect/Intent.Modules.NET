using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.ScopedMediatorTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.Oidc.Services
{
    public class ScopedMediator : IScopedMediator
    {
        private readonly IScopedExecutor _scopedExecutor;

        public ScopedMediator(IScopedExecutor scopedExecutor)
        {
            _scopedExecutor = scopedExecutor;
        }

        public async Task<TResponse> Send<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            return await _scopedExecutor.ExecuteAsync(
                async provider =>
                {
                    var mediator = provider.GetRequiredService<IMediator>();
                    return await mediator.Send(request, cancellationToken);
                });
        }

        public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest
        {
            await _scopedExecutor.ExecuteAsync(
                async provider =>
                {
                    var mediator = provider.GetRequiredService<IMediator>();
                    await mediator.Send(request, cancellationToken);
                });
        }

        public async Task<object?> Send(object request, CancellationToken cancellationToken = default)
        {
            return await _scopedExecutor.ExecuteAsync(
                async provider =>
                {
                    var mediator = provider.GetRequiredService<IMediator>();
                    return await mediator.Send(request, cancellationToken);
                });
        }

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
            IStreamRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            return _scopedExecutor.ExecuteStreamAsync(
                provider =>
                {
                    var mediator = provider.GetRequiredService<IMediator>();
                    return mediator.CreateStream(request, cancellationToken);
                });
        }

        public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
        {
            return _scopedExecutor.ExecuteStreamAsync(
                provider =>
                {
                    var mediator = provider.GetRequiredService<IMediator>();
                    return mediator.CreateStream(request, cancellationToken);
                });
        }
    }
}