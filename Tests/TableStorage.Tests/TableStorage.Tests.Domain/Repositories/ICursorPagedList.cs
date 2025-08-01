using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CursorPagedListInterface", Version = "1.0")]

namespace TableStorage.Tests.Domain.Repositories
{
    public interface ICursorPagedList<T> : IList<T>
    {
        int PageSize { get; }
        string? CursorToken { get; }
    }
}