using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Entities;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases.CreatePurchase
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreatePurchaseCommandHandler : IRequestHandler<CreatePurchaseCommand, PurchaseDto>
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public CreatePurchaseCommandHandler(IPurchaseRepository purchaseRepository, IMapper mapper)
        {
            _purchaseRepository = purchaseRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PurchaseDto> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
        {
            var newPurchase = new Purchase
            {
                Cost = CreateMoney(request.Cost),
            };

            _purchaseRepository.Add(newPurchase);
            await _purchaseRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newPurchase.MapToPurchaseDto(_mapper);
        }

        [IntentManaged(Mode.Fully)]
        public static Money CreateMoney(CreatePurchaseMoneyDto dto)
        {
            return new Money(amount: dto.Amount, currency: dto.Currency);
        }
    }
}