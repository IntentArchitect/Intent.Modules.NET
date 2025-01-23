using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared.Baskets;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Common.Eventing;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Common.Exceptions;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.UpdateBasket
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateBasketCommandHandler : IRequestHandler<UpdateBasketCommand>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public UpdateBasketCommandHandler(IBasketRepository basketRepository, IEventBus eventBus)
        {
            _basketRepository = basketRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateBasketCommand request, CancellationToken cancellationToken)
        {
            var basket = await _basketRepository.FindByIdAsync(request.Id, cancellationToken);
            if (basket is null)
            {
                throw new NotFoundException($"Could not find Basket '{request.Id}'");
            }

            basket.Number = request.Number;
            _eventBus.Publish(new BasketUpdatedEvent
            {
                Id = basket.Id,
                Number = basket.Number,
                BasketItems = basket.BasketItems
                    .Select(bi => new BasketItemDto
                    {
                        Id = bi.Id,
                        Description = bi.Description,
                        Amount = bi.Amount,
                        BasketId = bi.BasketId
                    })
                    .ToList()
            });

        }
    }
}