using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using GraphQL.CQRS.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Invoices
{
    public static class InvoiceCustomerDtoMappingExtensions
    {
        public static InvoiceCustomerDto MapToInvoiceCustomerDto(this Customer projectFrom, IMapper mapper)
        {
            return mapper.Map<InvoiceCustomerDto>(projectFrom);
        }

        public static List<InvoiceCustomerDto> MapToInvoiceCustomerDtoList(this IEnumerable<Customer> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToInvoiceCustomerDto(mapper)).ToList();
        }
    }
}