using AspNetCoreCleanArchitecture.Sample.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Products
{
    public class ProductDtoProfile : Profile
    {
        public ProductDtoProfile()
        {
            CreateMap<Product, ProductDto>();
        }
    }

    public static class ProductDtoMappingExtensions
    {
        public static ProductDto MapToProductDto(this Product projectFrom, IMapper mapper) => mapper.Map<ProductDto>(projectFrom);

        public static List<ProductDto> MapToProductDtoList(this IEnumerable<Product> projectFrom, IMapper mapper) => projectFrom.Select(x => x.MapToProductDto(mapper)).ToList();
    }
}