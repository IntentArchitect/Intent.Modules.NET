using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingDaprInvoices
{
    public static class DaprInvoiceDtoMappingExtensions
    {
        public static DaprInvoiceDto MapToDaprInvoiceDto(this DaprInvoice projectFrom, IMapper mapper)
            => mapper.Map<DaprInvoiceDto>(projectFrom);

        public static List<DaprInvoiceDto> MapToDaprInvoiceDtoList(this IEnumerable<DaprInvoice> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToDaprInvoiceDto(mapper)).ToList();
    }
}