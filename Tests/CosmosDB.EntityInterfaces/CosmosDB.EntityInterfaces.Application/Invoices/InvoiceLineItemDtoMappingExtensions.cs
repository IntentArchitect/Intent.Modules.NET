using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Invoices
{
    public static class InvoiceLineItemDtoMappingExtensions
    {
        public static InvoiceLineItemDto MapToInvoiceLineItemDto(this ILineItem projectFrom, IMapper mapper)
            => mapper.Map<InvoiceLineItemDto>(projectFrom);

        public static List<InvoiceLineItemDto> MapToInvoiceLineItemDtoList(this IEnumerable<ILineItem> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToInvoiceLineItemDto(mapper)).ToList();
    }
}