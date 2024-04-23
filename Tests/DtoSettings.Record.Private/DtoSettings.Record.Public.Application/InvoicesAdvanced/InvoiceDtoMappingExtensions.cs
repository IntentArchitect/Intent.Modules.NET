using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DtoSettings.Record.Public.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.InvoicesAdvanced
{
    public static class InvoiceDtoMappingExtensions
    {
        public static InvoiceDto MapToInvoiceDto(this Invoice projectFrom, IMapper mapper)
            => mapper.Map<InvoiceDto>(projectFrom);

        public static List<InvoiceDto> MapToInvoiceDtoList(this IEnumerable<Invoice> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToInvoiceDto(mapper)).ToList();
    }
}