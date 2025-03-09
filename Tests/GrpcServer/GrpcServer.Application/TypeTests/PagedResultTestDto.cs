using System.Collections.Generic;
using GrpcServer.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class PagedResultTestDto
    {
        public PagedResultTestDto()
        {
            PagedResultField = null!;
            PagedResultFieldCollection = null!;
        }

        public PagedResult<ComplexTypeDto> PagedResultField { get; set; }
        public List<PagedResult<ComplexTypeDto>> PagedResultFieldCollection { get; set; }
        public PagedResult<ComplexTypeDto> PagedResultFieldNullable { get; set; }
        public List<PagedResult<ComplexTypeDto>> PagedResultFieldNullableCollection { get; set; }

        public static PagedResultTestDto Create(
            PagedResult<ComplexTypeDto> pagedResultField,
            List<PagedResult<ComplexTypeDto>> pagedResultFieldCollection,
            PagedResult<ComplexTypeDto> pagedResultFieldNullable,
            List<PagedResult<ComplexTypeDto>> pagedResultFieldNullableCollection)
        {
            return new PagedResultTestDto
            {
                PagedResultField = pagedResultField,
                PagedResultFieldCollection = pagedResultFieldCollection,
                PagedResultFieldNullable = pagedResultFieldNullable,
                PagedResultFieldNullableCollection = pagedResultFieldNullableCollection
            };
        }
    }
}