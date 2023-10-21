using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.EfInvoices
{
    public static class EfInvoiceEfLineDtoMappingExtensions
    {
        public static EfInvoiceEfLineDto MapToEfInvoiceEfLineDto(this EfLine projectFrom, IMapper mapper)
            => mapper.Map<EfInvoiceEfLineDto>(projectFrom);

        public static List<EfInvoiceEfLineDto> MapToEfInvoiceEfLineDtoList(this IEnumerable<EfLine> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToEfInvoiceEfLineDto(mapper)).ToList();
    }
}