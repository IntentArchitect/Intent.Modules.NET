using AspNetCoreCleanArchitecture.Sample.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Orders
{
    public class OrderOrderOrderLineDtoProfile : Profile
    {
        public OrderOrderOrderLineDtoProfile()
        {
            CreateMap<OrderLine, OrderOrderOrderLineDto>()
                .ForMember(d => d.Discount, opt => opt.MapFrom(src => (decimal)src.Discount))
                .ForMember(d => d.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(d => d.ProductDescription, opt => opt.MapFrom(src => src.Product.Description))
                .ForMember(d => d.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(d => d.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl));
        }
    }

    public static class OrderOrderOrderLineDtoMappingExtensions
    {
        public static OrderOrderOrderLineDto MapToOrderOrderOrderLineDto(this OrderLine projectFrom, IMapper mapper) => mapper.Map<OrderOrderOrderLineDto>(projectFrom);

        public static List<OrderOrderOrderLineDto> MapToOrderOrderOrderLineDtoList(
            this IEnumerable<OrderLine> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToOrderOrderOrderLineDto(mapper)).ToList();
    }
}