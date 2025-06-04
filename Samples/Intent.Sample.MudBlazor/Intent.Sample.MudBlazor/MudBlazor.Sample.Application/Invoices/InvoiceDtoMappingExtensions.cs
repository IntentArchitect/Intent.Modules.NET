using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MudBlazor.Sample.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MudBlazor.Sample.Application.Invoices
{
    public static class InvoiceDtoMappingExtensions
    {
        public static InvoiceDto MapToInvoiceDto(this Invoice projectFrom, IMapper mapper)
            => mapper.Map<InvoiceDto>(projectFrom);

        public static List<InvoiceDto> MapToInvoiceDtoList(this IEnumerable<Invoice> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToInvoiceDto(mapper)).ToList();
    }
}