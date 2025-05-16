using Intent.Modules.NET.Tests.Domain.Core.Common.Exceptions;
using Intent.Modules.NET.Tests.Module1.Application.Contracts.Products.DeleteProduct;
using Intent.Modules.NET.Tests.Module1.Domain.Common.Interfaces;
using Intent.Modules.NET.Tests.Module1.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.Products.DeleteProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        [IntentManaged(Mode.Merge)]
        public DeleteProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.FindByIdAsync(request.Id, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Could not find Product '{request.Id}'");
            }

            _productRepository.Remove(product);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}