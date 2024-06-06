using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingEfInvoices
{
    public static class EfInvoiceDtoMappingExtensions
    {
        public static EfInvoiceDto MapToEfInvoiceDto(this EfInvoice projectFrom, IMapper mapper)
            => mapper.Map<EfInvoiceDto>(projectFrom);

        public static List<EfInvoiceDto> MapToEfInvoiceDtoList(this IEnumerable<EfInvoice> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToEfInvoiceDto(mapper)).ToList();
    }
}