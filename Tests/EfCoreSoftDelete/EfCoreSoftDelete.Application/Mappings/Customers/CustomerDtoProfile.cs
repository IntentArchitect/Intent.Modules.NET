using AutoMapper;
using EfCoreSoftDelete.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace EfCoreSoftDelete.Application.Customers
{
    public class CustomerDtoProfile : Profile
    {
        public CustomerDtoProfile()
        {
            CreateMap<Customer, CustomerDto>()
                .ForMember(d => d.OtherAddresses, opt => opt.MapFrom(src => src.OtherAddresses));
        }
    }

    public static class CustomerDtoMappingExtensions
    {
        public static CustomerDto MapToCustomerDto(this Customer projectFrom, IMapper mapper) => mapper.Map<CustomerDto>(projectFrom);

        public static List<CustomerDto> MapToCustomerDtoList(this IEnumerable<Customer> projectFrom, IMapper mapper) => projectFrom.Select(x => x.MapToCustomerDto(mapper)).ToList();
    }
}