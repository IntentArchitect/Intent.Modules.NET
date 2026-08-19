using AzureIdentityManagement.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Azure.TableStorage.CursorPagedList", Version = "1.0")]

namespace AzureIdentityManagement.Application.Common.Pagination
{
    public class CursorPagedList<T> : List<T>, ICursorPagedList<T>
    {
        public CursorPagedList(string? cursorToken, int pageSize, IEnumerable<T> results)
        {
            CursorToken = cursorToken;
            PageSize = pageSize;
            AddRange(results);
        }

        public string? CursorToken { get; private set; }
        public int PageSize { get; private set; }
        public bool HasMoreResults => !string.IsNullOrEmpty(CursorToken);
    }
}