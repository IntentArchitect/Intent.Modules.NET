using AutoMapper;
using EfCoreSoftDelete.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace EfCoreSoftDelete.Application.Customers
{
    public class CustomerAddressBuildingDtoProfile : Profile
    {
        public CustomerAddressBuildingDtoProfile()
        {
            CreateMap<AddressBuilding, CustomerAddressBuildingDto>();
        }
    }

    public static class CustomerAddressBuildingDtoMappingExtensions
    {
        public static CustomerAddressBuildingDto MapToCustomerAddressBuildingDto(
            this AddressBuilding projectFrom,
            IMapper mapper) => mapper.Map<CustomerAddressBuildingDto>(projectFrom);

        public static List<CustomerAddressBuildingDto> MapToCustomerAddressBuildingDtoList(
            this IEnumerable<AddressBuilding> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToCustomerAddressBuildingDto(mapper)).ToList();
    }
}