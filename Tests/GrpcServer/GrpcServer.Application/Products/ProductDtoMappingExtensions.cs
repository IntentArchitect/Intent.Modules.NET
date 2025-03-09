using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GrpcServer.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace GrpcServer.Application.Products
{
    public static class ProductDtoMappingExtensions
    {
        public static ProductDto MapToProductDto(this Product projectFrom, IMapper mapper)
            => mapper.Map<ProductDto>(projectFrom);

        public static List<ProductDto> MapToProductDtoList(this IEnumerable<Product> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToProductDto(mapper)).ToList();
    }
}