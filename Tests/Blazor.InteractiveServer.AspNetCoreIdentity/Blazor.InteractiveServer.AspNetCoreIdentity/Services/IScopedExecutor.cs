using System.Security.Claims;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.ScopedExecutorInterfaceTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.AspNetCoreIdentity.Services
{
    public interface IScopedExecutor
    {
        Task ExecuteAsync(Func<IServiceProvider, Task> action);
        Task<T> ExecuteAsync<T>(Func<IServiceProvider, Task<T>> action);
        IAsyncEnumerable<T> ExecuteStreamAsync<T>(Func<IServiceProvider, IAsyncEnumerable<T>> action, CancellationToken cancellationToken = default);
    }

    public interface ISetCurrentUserContext
    {
        void SetContext(ClaimsPrincipal principal);
    }
}