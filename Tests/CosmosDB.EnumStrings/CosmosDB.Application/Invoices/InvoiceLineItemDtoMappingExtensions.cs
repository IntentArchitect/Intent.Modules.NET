using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.Application.Invoices
{
    public static class InvoiceLineItemDtoMappingExtensions
    {
        public static InvoiceLineItemDto MapToInvoiceLineItemDto(this LineItem projectFrom, IMapper mapper)
            => mapper.Map<InvoiceLineItemDto>(projectFrom);

        public static List<InvoiceLineItemDto> MapToInvoiceLineItemDtoList(this IEnumerable<LineItem> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToInvoiceLineItemDto(mapper)).ToList();
    }
}