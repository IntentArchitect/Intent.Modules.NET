using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.AzureFunction.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Application.Products.DeleteProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ProductDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public DeleteProductCommandHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ProductDto> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var existingProduct = await _productRepository.FindByIdAsync(request.Id, cancellationToken);
            _productRepository.Remove(existingProduct);
            return existingProduct.MapToProductDto(_mapper);
        }
    }
}