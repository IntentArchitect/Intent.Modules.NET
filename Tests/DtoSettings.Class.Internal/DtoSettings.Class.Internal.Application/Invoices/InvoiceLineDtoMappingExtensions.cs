using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DtoSettings.Class.Internal.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace DtoSettings.Class.Internal.Application.Invoices
{
    public static class InvoiceLineDtoMappingExtensions
    {
        public static InvoiceLineDto MapToInvoiceLineDto(this InvoiceLine projectFrom, IMapper mapper)
            => mapper.Map<InvoiceLineDto>(projectFrom);

        public static List<InvoiceLineDto> MapToInvoiceLineDtoList(this IEnumerable<InvoiceLine> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToInvoiceLineDto(mapper)).ToList();
    }
}