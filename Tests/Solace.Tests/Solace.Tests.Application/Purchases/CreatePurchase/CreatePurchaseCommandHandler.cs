using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Solace.Tests.Application.Common.Eventing;
using Solace.Tests.Domain.Entities;
using Solace.Tests.Domain.Repositories;
using Solace.Tests.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Solace.Tests.Application.Purchases.CreatePurchase
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreatePurchaseCommandHandler : IRequestHandler<CreatePurchaseCommand, Guid>
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public CreatePurchaseCommandHandler(IPurchaseRepository purchaseRepository, IEventBus eventBus)
        {
            _purchaseRepository = purchaseRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
        {
            var purchase = new Purchase
            {
                AccountId = request.AccountId,
                Amount = request.Amount
            };

            _purchaseRepository.Add(purchase);
            await _purchaseRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _eventBus.Send(new PurchaseCreated
            {
                Id = purchase.Id,
                AccountId = purchase.AccountId,
                Amount = purchase.Amount
            });
            return purchase.Id;
        }
    }
}