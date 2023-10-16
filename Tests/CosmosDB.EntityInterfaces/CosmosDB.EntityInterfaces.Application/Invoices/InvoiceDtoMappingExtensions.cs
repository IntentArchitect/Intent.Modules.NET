using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Invoices
{
    public static class InvoiceDtoMappingExtensions
    {
        public static InvoiceDto MapToInvoiceDto(this IInvoice projectFrom, IMapper mapper)
            => mapper.Map<InvoiceDto>(projectFrom);

        public static List<InvoiceDto> MapToInvoiceDtoList(this IEnumerable<IInvoice> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToInvoiceDto(mapper)).ToList();
    }
}