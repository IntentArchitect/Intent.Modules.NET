using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Solace.Tests.Domain.Common.Exceptions;
using Solace.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Solace.Tests.Application.Purchases.UpdatePurchase
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdatePurchaseCommandHandler : IRequestHandler<UpdatePurchaseCommand>
    {
        private readonly IPurchaseRepository _purchaseRepository;

        [IntentManaged(Mode.Merge)]
        public UpdatePurchaseCommandHandler(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdatePurchaseCommand request, CancellationToken cancellationToken)
        {
            var purchase = await _purchaseRepository.FindByIdAsync(request.Id, cancellationToken);
            if (purchase is null)
            {
                throw new NotFoundException($"Could not find Purchase '{request.Id}'");
            }

            purchase.AccountId = request.AccountId;
            purchase.Amount = request.Amount;
        }
    }
}