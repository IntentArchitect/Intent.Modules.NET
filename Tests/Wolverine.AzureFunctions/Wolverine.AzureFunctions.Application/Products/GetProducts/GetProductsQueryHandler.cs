using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Wolverine.AzureFunctions.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryHandler", Version = "1.0")]

namespace Wolverine.AzureFunctions.Application.Products.GetProducts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetProductsQueryHandler
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Merge, Signature = Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ProductDto>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            var products = await _productRepository.FindAllAsync(cancellationToken);
            return products.MapToProductDtoList(_mapper);
        }
    }
}