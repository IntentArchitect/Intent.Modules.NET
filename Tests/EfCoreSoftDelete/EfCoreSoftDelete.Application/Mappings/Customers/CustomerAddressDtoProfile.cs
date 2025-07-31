using AutoMapper;
using EfCoreSoftDelete.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace EfCoreSoftDelete.Application.Customers
{
    public class CustomerAddressDtoProfile : Profile
    {
        public CustomerAddressDtoProfile()
        {
            CreateMap<Address, CustomerAddressDto>()
                .ForMember(d => d.OtherBuildings, opt => opt.MapFrom(src => src.OtherBuildings));
        }
    }

    public static class CustomerAddressDtoMappingExtensions
    {
        public static CustomerAddressDto MapToCustomerAddressDto(this Address projectFrom, IMapper mapper) => mapper.Map<CustomerAddressDto>(projectFrom);

        public static List<CustomerAddressDto> MapToCustomerAddressDtoList(
            this IEnumerable<Address> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToCustomerAddressDto(mapper)).ToList();
    }
}