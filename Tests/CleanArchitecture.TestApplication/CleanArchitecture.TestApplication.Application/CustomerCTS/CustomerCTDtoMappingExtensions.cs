using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Entities.ComplexTypes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.CustomerCTS
{
    public static class CustomerCTDtoMappingExtensions
    {
        public static CustomerCTDto MapToCustomerCTDto(this CustomerCT projectFrom, IMapper mapper)
            => mapper.Map<CustomerCTDto>(projectFrom);

        public static List<CustomerCTDto> MapToCustomerCTDtoList(this IEnumerable<CustomerCT> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCustomerCTDto(mapper)).ToList();
    }
}