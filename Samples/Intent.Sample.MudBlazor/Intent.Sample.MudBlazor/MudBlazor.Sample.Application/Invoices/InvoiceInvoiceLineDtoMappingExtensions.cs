using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MudBlazor.Sample.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MudBlazor.Sample.Application.Invoices
{
    public static class InvoiceInvoiceLineDtoMappingExtensions
    {
        public static InvoiceInvoiceLineDto MapToInvoiceInvoiceLineDto(this InvoiceLine projectFrom, IMapper mapper)
            => mapper.Map<InvoiceInvoiceLineDto>(projectFrom);

        public static List<InvoiceInvoiceLineDto> MapToInvoiceInvoiceLineDtoList(this IEnumerable<InvoiceLine> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToInvoiceInvoiceLineDto(mapper)).ToList();
    }
}