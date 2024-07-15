using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Products.CreateProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, string>
    {
        private readonly IProductRepository _productRepository;

        [IntentManaged(Mode.Merge)]
        public CreateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description
            };

            _productRepository.Add(product);
            await _productRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return product.Id;
        }
    }
}