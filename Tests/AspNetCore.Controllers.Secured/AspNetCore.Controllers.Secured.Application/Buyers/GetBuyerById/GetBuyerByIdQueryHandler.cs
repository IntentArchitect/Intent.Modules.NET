using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Controllers.Secured.Domain.Common.Exceptions;
using AspNetCore.Controllers.Secured.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AspNetCore.Controllers.Secured.Application.Buyers.GetBuyerById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetBuyerByIdQueryHandler : IRequestHandler<GetBuyerByIdQuery, BuyerDto>
    {
        private readonly IBuyerRepository _buyerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetBuyerByIdQueryHandler(IBuyerRepository buyerRepository, IMapper mapper)
        {
            _buyerRepository = buyerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<BuyerDto> Handle(GetBuyerByIdQuery request, CancellationToken cancellationToken)
        {
            var buyer = await _buyerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (buyer is null)
            {
                throw new NotFoundException($"Could not find Buyer '{request.Id}'");
            }

            return buyer.MapToBuyerDto(_mapper);
        }
    }
}