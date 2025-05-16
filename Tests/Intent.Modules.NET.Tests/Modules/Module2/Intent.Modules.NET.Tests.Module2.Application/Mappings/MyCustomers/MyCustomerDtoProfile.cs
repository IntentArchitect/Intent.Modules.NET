using AutoMapper;
using Intent.Modules.NET.Tests.Module2.Application.Contracts.MyCustomers;
using Intent.Modules.NET.Tests.Module2.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module2.Application.MyCustomers
{
    public class MyCustomerDtoProfile : Profile
    {
        public MyCustomerDtoProfile()
        {
            CreateMap<MyCustomer, MyCustomerDto>();
        }
    }

    public static class MyCustomerDtoMappingExtensions
    {
        public static MyCustomerDto MapToMyCustomerDto(this MyCustomer projectFrom, IMapper mapper) => mapper.Map<MyCustomerDto>(projectFrom);

        public static List<MyCustomerDto> MapToMyCustomerDtoList(this IEnumerable<MyCustomer> projectFrom, IMapper mapper) => projectFrom.Select(x => x.MapToMyCustomerDto(mapper)).ToList();
    }
}