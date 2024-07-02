using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.RichProducts
{
    public static class RichProductDtoMappingExtensions
    {
        public static RichProductDto MapToRichProductDto(this RichProduct projectFrom, IMapper mapper)
            => mapper.Map<RichProductDto>(projectFrom);

        public static List<RichProductDto> MapToRichProductDtoList(this IEnumerable<RichProduct> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToRichProductDto(mapper)).ToList();
    }
}