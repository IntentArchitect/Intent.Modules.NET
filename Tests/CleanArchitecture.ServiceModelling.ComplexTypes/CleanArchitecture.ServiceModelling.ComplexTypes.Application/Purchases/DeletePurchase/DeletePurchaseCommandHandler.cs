using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Common.Exceptions;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases.DeletePurchase
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeletePurchaseCommandHandler : IRequestHandler<DeletePurchaseCommand>
    {
        private readonly IPurchaseRepository _purchaseRepository;

        [IntentManaged(Mode.Merge)]
        public DeletePurchaseCommandHandler(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(DeletePurchaseCommand request, CancellationToken cancellationToken)
        {
            var existingPurchase = await _purchaseRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingPurchase is null)
            {
                throw new NotFoundException($"Could not find Purchase '{request.Id}' ");
            }
            _purchaseRepository.Remove(existingPurchase);
            return Unit.Value;
        }
    }
}