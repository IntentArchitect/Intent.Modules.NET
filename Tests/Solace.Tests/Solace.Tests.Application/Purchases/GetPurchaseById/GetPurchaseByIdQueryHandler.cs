using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Solace.Tests.Domain.Common.Exceptions;
using Solace.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Solace.Tests.Application.Purchases.GetPurchaseById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPurchaseByIdQueryHandler : IRequestHandler<GetPurchaseByIdQuery, PurchaseDto>
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetPurchaseByIdQueryHandler(IPurchaseRepository purchaseRepository, IMapper mapper)
        {
            _purchaseRepository = purchaseRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PurchaseDto> Handle(GetPurchaseByIdQuery request, CancellationToken cancellationToken)
        {
            var purchase = await _purchaseRepository.FindByIdAsync(request.Id, cancellationToken);
            if (purchase is null)
            {
                throw new NotFoundException($"Could not find Purchase '{request.Id}'");
            }
            return purchase.MapToPurchaseDto(_mapper);
        }
    }
}