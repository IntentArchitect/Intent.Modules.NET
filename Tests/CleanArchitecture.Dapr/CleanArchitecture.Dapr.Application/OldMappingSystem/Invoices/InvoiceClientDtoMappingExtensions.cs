using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Dapr.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices
{
    public static class InvoiceClientDtoMappingExtensions
    {
        public static InvoiceClientDto MapToInvoiceClientDto(this Client projectFrom, IMapper mapper)
            => mapper.Map<InvoiceClientDto>(projectFrom);

        public static List<InvoiceClientDto> MapToInvoiceClientDtoList(this IEnumerable<Client> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToInvoiceClientDto(mapper)).ToList();
    }
}