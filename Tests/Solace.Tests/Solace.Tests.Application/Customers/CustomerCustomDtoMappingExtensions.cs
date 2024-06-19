using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Solace.Tests.Domain.Contracts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Solace.Tests.Application.Customers
{
    public static class CustomerCustomDtoMappingExtensions
    {
        public static CustomerCustomDto MapToCustomerCustomDto(this CustomerCustom projectFrom, IMapper mapper)
            => mapper.Map<CustomerCustomDto>(projectFrom);

        public static List<CustomerCustomDto> MapToCustomerCustomDtoList(this IEnumerable<CustomerCustom> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCustomerCustomDto(mapper)).ToList();
    }
}