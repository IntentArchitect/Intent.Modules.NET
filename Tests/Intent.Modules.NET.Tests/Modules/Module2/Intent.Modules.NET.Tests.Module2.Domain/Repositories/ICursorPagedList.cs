using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CursorPagedListInterface", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module2.Domain.Repositories
{
    public interface ICursorPagedList<T> : IList<T>
    {
        int PageSize { get; }
        string? CursorToken { get; }
    }
}