using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.Sample.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace MudBlazor.Sample.Application.Products.GetProductsLookup
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetProductsLookupQueryHandler : IRequestHandler<GetProductsLookupQuery, List<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetProductsLookupQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ProductDto>> Handle(GetProductsLookupQuery request, CancellationToken cancellationToken)
        {
            var entity = await _productRepository.FindAllAsync(cancellationToken);
            return entity.MapToProductDtoList(_mapper);
        }
    }
}