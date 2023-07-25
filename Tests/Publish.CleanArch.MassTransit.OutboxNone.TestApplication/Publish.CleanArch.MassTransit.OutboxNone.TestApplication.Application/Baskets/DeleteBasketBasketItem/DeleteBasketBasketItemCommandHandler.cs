using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Common.Eventing;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Common.Exceptions;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Entities;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.DeleteBasketBasketItem
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteBasketBasketItemCommandHandler : IRequestHandler<DeleteBasketBasketItemCommand>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public DeleteBasketBasketItemCommandHandler(IBasketRepository basketRepository, IEventBus eventBus)
        {
            _basketRepository = basketRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteBasketBasketItemCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _basketRepository.FindByIdAsync(request.BasketId, cancellationToken);

            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(Basket)} of Id '{request.BasketId}' could not be found");
            }

            var existingBasketItem = aggregateRoot.BasketItems.FirstOrDefault(p => p.Id == request.Id);

            if (existingBasketItem is null)
            {
                throw new NotFoundException($"{nameof(BasketItem)} of Id '{request.Id}' could not be found associated with {nameof(Basket)} of Id '{request.BasketId}'");
            }
            aggregateRoot.BasketItems.Remove(existingBasketItem);
            _eventBus.Publish(existingBasketItem.MapToBasketItemDeletedEvent());

        }
    }
}