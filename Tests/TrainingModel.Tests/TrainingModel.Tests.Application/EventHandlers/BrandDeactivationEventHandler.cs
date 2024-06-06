using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Application.Common.Models;
using TrainingModel.Tests.Domain.Events;
using TrainingModel.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DefaultDomainEventHandler", Version = "1.0")]

namespace TrainingModel.Tests.Application.EventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class BrandDeactivationEventHandler : INotificationHandler<DomainEventNotification<BrandDeactivationEvent>>
    {
        private readonly IProductRepository _productRepository;


        [IntentManaged(Mode.Merge)]
        public BrandDeactivationEventHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(
            DomainEventNotification<BrandDeactivationEvent> notification,
            CancellationToken cancellationToken)
        {
            var products = await _productRepository.FindAllAsync(p => p.BrandId == notification.DomainEvent.Brand.Id, cancellationToken);
            foreach (var product in products)
            {
                product.DeActivate();
            }
        }
    }
}