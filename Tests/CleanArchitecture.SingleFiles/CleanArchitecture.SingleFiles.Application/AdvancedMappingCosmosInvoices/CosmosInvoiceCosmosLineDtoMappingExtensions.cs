using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingCosmosInvoices
{
    public static class CosmosInvoiceCosmosLineDtoMappingExtensions
    {
        public static CosmosInvoiceCosmosLineDto MapToCosmosInvoiceCosmosLineDto(this CosmosLine projectFrom, IMapper mapper)
            => mapper.Map<CosmosInvoiceCosmosLineDto>(projectFrom);

        public static List<CosmosInvoiceCosmosLineDto> MapToCosmosInvoiceCosmosLineDtoList(this IEnumerable<CosmosLine> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCosmosInvoiceCosmosLineDto(mapper)).ToList();
    }
}