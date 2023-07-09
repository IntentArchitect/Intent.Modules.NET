using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Common.Eventing;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Common.Exceptions;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.DeleteBasket
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteBasketCommandHandler : IRequestHandler<DeleteBasketCommand>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public DeleteBasketCommandHandler(IBasketRepository basketRepository, IEventBus eventBus)
        {
            _basketRepository = basketRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(DeleteBasketCommand request, CancellationToken cancellationToken)
        {
            var existingBasket = await _basketRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingBasket is null)
            {
                throw new NotFoundException($"Could not find Basket '{request.Id}' ");
            }
            _basketRepository.Remove(existingBasket);
            _eventBus.Publish(existingBasket.MapToBasketDeletedEvent());
            return Unit.Value;
        }
    }
}