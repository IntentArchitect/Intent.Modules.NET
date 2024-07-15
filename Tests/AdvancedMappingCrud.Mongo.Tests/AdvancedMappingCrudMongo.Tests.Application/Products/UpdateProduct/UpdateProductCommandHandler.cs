using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrudMongo.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Products.UpdateProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IProductRepository _productRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.FindByIdAsync(request.Id, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Could not find Product '{request.Id}'");
            }

            product.Name = request.Name;
            product.Description = request.Description;

            _productRepository.Update(product);
        }
    }
}