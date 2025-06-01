using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FastEndpointsTest.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace FastEndpointsTest.Application.SimpleProducts
{
    public static class SimpleProductDtoMappingExtensions
    {
        public static SimpleProductDto MapToSimpleProductDto(this SimpleProduct projectFrom, IMapper mapper)
            => mapper.Map<SimpleProductDto>(projectFrom);

        public static List<SimpleProductDto> MapToSimpleProductDtoList(this IEnumerable<SimpleProduct> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToSimpleProductDto(mapper)).ToList();
    }
}