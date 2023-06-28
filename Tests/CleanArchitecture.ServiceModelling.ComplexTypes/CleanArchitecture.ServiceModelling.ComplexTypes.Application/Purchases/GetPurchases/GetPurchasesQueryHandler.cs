using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases.GetPurchases
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPurchasesQueryHandler : IRequestHandler<GetPurchasesQuery, List<PurchaseDto>>
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetPurchasesQueryHandler(IPurchaseRepository purchaseRepository, IMapper mapper)
        {
            _purchaseRepository = purchaseRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<PurchaseDto>> Handle(GetPurchasesQuery request, CancellationToken cancellationToken)
        {
            var purchases = await _purchaseRepository.FindAllAsync(cancellationToken);
            return purchases.MapToPurchaseDtoList(_mapper);
        }
    }
}