using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Cosmos.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders
{
    public static class OrderOrderTagsDtoMappingExtensions
    {
        public static OrderOrderTagsDto MapToOrderOrderTagsDto(this OrderTags projectFrom, IMapper mapper)
            => mapper.Map<OrderOrderTagsDto>(projectFrom);

        public static List<OrderOrderTagsDto> MapToOrderOrderTagsDtoList(this IEnumerable<OrderTags> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOrderOrderTagsDto(mapper)).ToList();
    }
}