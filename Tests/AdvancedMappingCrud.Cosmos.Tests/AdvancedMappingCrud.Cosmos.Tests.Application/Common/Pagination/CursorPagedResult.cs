using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Pagination.CursorPagedResult", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Common.Pagination
{
    public class CursorPagedResult<T>
    {
        public CursorPagedResult()
        {
            Data = null!;
        }

        public IEnumerable<T> Data { get; set; }
        public string? CursorToken { get; set; }
        public int PageSize { get; set; }
        public bool HasMoreResults => !string.IsNullOrEmpty(CursorToken);

        public static CursorPagedResult<T> Create(int pageSize, string? cursorToken, IEnumerable<T> data)
        {
            return new CursorPagedResult<T>
            {
                CursorToken = cursorToken,
                PageSize = pageSize,
                Data = data
            };
        }
    }
}