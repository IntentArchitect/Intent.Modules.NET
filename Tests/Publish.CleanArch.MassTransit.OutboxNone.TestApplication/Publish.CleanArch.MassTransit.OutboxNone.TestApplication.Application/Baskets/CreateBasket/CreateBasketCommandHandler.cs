using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared.Baskets;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Common.Eventing;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Entities;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.CreateBasket
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateBasketCommandHandler : IRequestHandler<CreateBasketCommand, Guid>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public CreateBasketCommandHandler(IBasketRepository basketRepository, IEventBus eventBus)
        {
            _basketRepository = basketRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateBasketCommand request, CancellationToken cancellationToken)
        {
            var basket = new Basket
            {
                Number = request.Number
            };

            _basketRepository.Add(basket);
            await _basketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _eventBus.Publish(new BasketCreatedEvent
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
            return basket.Id;
        }
    }
}