using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.NoEventing.Domain.Common.Exceptions;
using TestMediator.NoEventing.Domain.Repositories.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.QueryHandler", Version = "1.0")]

namespace TestMediator.NoEventing.Application.Products.GetProductById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IProductRepository _productRepository;

        [IntentManaged(Mode.Merge)]
        public GetProductByIdQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async ValueTask<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return await _productRepository.FindByIdProjectToAsync<ProductDto>(request.Id, cancellationToken)
                ?? throw new NotFoundException($"Could not find Product '{request.Id}'");
        }
    }
}
