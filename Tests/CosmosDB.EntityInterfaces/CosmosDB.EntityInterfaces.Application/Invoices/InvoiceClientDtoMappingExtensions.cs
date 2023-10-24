using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Invoices
{
    public static class InvoiceClientDtoMappingExtensions
    {
        public static InvoiceClientDto MapToInvoiceClientDto(this IClient projectFrom, IMapper mapper)
            => mapper.Map<InvoiceClientDto>(projectFrom);

        public static List<InvoiceClientDto> MapToInvoiceClientDtoList(this IEnumerable<IClient> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToInvoiceClientDto(mapper)).ToList();
    }
}