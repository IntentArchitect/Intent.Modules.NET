using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.DaprInvoices
{
    public static class DaprInvoiceDaprLineDtoMappingExtensions
    {
        public static DaprInvoiceDaprLineDto MapToDaprInvoiceDaprLineDto(this DaprLine projectFrom, IMapper mapper)
            => mapper.Map<DaprInvoiceDaprLineDto>(projectFrom);

        public static List<DaprInvoiceDaprLineDto> MapToDaprInvoiceDaprLineDtoList(this IEnumerable<DaprLine> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToDaprInvoiceDaprLineDto(mapper)).ToList();
    }
}