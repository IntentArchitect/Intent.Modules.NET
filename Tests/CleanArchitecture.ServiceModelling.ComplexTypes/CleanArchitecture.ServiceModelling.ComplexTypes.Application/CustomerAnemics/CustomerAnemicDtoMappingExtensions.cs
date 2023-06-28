using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerAnemics
{
    public static class CustomerAnemicDtoMappingExtensions
    {
        public static CustomerAnemicDto MapToCustomerAnemicDto(this CustomerAnemic projectFrom, IMapper mapper)
            => mapper.Map<CustomerAnemicDto>(projectFrom);

        public static List<CustomerAnemicDto> MapToCustomerAnemicDtoList(this IEnumerable<CustomerAnemic> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCustomerAnemicDto(mapper)).ToList();
    }
}