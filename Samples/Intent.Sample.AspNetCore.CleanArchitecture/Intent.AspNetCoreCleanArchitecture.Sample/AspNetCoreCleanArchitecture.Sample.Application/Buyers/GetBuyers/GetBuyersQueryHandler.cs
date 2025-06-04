using AspNetCoreCleanArchitecture.Sample.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers.GetBuyers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetBuyersQueryHandler : IRequestHandler<GetBuyersQuery, List<BuyerDto>>
    {
        private readonly IBuyerRepository _buyerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetBuyersQueryHandler(IBuyerRepository buyerRepository, IMapper mapper)
        {
            _buyerRepository = buyerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<BuyerDto>> Handle(GetBuyersQuery request, CancellationToken cancellationToken)
        {
            var buyers = await _buyerRepository.FindAllAsync(cancellationToken);
            return buyers.MapToBuyerDtoList(_mapper);
        }
    }
}