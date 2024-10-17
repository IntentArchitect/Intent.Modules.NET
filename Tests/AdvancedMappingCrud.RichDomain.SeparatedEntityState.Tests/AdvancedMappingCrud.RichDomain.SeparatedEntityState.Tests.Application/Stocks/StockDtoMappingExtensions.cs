using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Stocks
{
    public static class StockDtoMappingExtensions
    {
        public static StockDto MapToStockDto(this Stock projectFrom, IMapper mapper)
            => mapper.Map<StockDto>(projectFrom);

        public static List<StockDto> MapToStockDtoList(this IEnumerable<Stock> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToStockDto(mapper)).ToList();
    }
}