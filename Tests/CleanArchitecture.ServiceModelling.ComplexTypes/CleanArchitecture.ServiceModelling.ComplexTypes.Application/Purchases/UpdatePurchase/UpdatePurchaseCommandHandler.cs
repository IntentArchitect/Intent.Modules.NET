using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Common.Exceptions;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases.UpdatePurchase
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
            var existingPurchase = await _purchaseRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingPurchase is null)
            {
                throw new NotFoundException($"Could not find Purchase '{request.Id}'");
            }
            existingPurchase.Cost = CreateMoney(request.Cost);

        }

        [IntentManaged(Mode.Fully)]
        public static Money CreateMoney(UpdatePurchaseMoneyDto dto)
        {
            return new Money(amount: dto.Amount, currency: dto.Currency);
        }
    }
}