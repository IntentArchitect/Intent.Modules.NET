using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches
{
    public static class CustomerRichDtoMappingExtensions
    {
        public static CustomerRichDto MapToCustomerRichDto(this CustomerRich projectFrom, IMapper mapper)
            => mapper.Map<CustomerRichDto>(projectFrom);

        public static List<CustomerRichDto> MapToCustomerRichDtoList(this IEnumerable<CustomerRich> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCustomerRichDto(mapper)).ToList();
    }
}