using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Dapr.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices
{
    public static class InvoiceInvoiceLineDtoMappingExtensions
    {
        public static InvoiceInvoiceLineDto MapToInvoiceInvoiceLineDto(this InvoiceLine projectFrom, IMapper mapper)
            => mapper.Map<InvoiceInvoiceLineDto>(projectFrom);

        public static List<InvoiceInvoiceLineDto> MapToInvoiceInvoiceLineDtoList(this IEnumerable<InvoiceLine> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToInvoiceInvoiceLineDto(mapper)).ToList();
    }
}