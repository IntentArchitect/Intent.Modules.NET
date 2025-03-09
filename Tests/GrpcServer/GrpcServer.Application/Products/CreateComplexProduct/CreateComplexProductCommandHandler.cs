using System;
using System.Threading;
using System.Threading.Tasks;
using GrpcServer.Domain.Entities;
using GrpcServer.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace GrpcServer.Application.Products.CreateComplexProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateComplexProductCommandHandler : IRequestHandler<CreateComplexProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;

        [IntentManaged(Mode.Merge)]
        public CreateComplexProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateComplexProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name
            };

            _productRepository.Add(product);
            await _productRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return product.Id;
        }
    }
}