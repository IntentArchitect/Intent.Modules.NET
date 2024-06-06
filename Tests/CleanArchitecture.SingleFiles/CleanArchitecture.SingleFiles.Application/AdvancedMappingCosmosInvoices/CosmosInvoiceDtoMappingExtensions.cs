using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingCosmosInvoices
{
    public static class CosmosInvoiceDtoMappingExtensions
    {
        public static CosmosInvoiceDto MapToCosmosInvoiceDto(this CosmosInvoice projectFrom, IMapper mapper)
            => mapper.Map<CosmosInvoiceDto>(projectFrom);

        public static List<CosmosInvoiceDto> MapToCosmosInvoiceDtoList(this IEnumerable<CosmosInvoice> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCosmosInvoiceDto(mapper)).ToList();
    }
}