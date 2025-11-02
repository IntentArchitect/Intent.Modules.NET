using CleanArchitecture.QueueStorage.Application.Common.Eventing;
using CleanArchitecture.QueueStorage.Domain.Entities;
using CleanArchitecture.QueueStorage.Domain.Repositories;
using CleanArchitecture.QueueStorage.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.QueueStorage.Application.Products.CreateProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public CreateProductCommandHandler(IProductRepository productRepository, IEventBus eventBus)
        {
            _productRepository = productRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Qty = request.Qty
            };

            _productRepository.Add(product);
            await _productRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _eventBus.Send(new Eventing.Messages.CreateProductCommand
            {
                Name = request.Name,
                Qty = request.Qty
            });
            _eventBus.Send(new CreateStockForProductCommand
            {
                Name = request.Name,
                Qty = request.Qty
            });
            return product.Id;
        }
    }
}