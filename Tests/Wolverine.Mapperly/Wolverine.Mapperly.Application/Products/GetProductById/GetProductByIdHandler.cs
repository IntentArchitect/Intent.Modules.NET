using Intent.RoslynWeaver.Attributes;
using Wolverine.Mapperly.Application.Mappings.Products;
using Wolverine.Mapperly.Domain.Common.Exceptions;
using Wolverine.Mapperly.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryHandler", Version = "1.0")]

namespace Wolverine.Mapperly.Application.Products.GetProductById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetProductByIdHandler
    {
        private readonly IProductRepository _productRepository;
        private readonly ProductDtoMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetProductByIdHandler(IProductRepository productRepository, ProductDtoMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Merge, Signature = Mode.Fully, Body = Mode.Fully)]
        public async Task<ProductDto> Handle(GetProductById request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.FindByIdAsync(request.Id, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Could not find Product '{request.Id}'");
            }
            return _mapper.ProductToProductDto(product);
        }
    }
}