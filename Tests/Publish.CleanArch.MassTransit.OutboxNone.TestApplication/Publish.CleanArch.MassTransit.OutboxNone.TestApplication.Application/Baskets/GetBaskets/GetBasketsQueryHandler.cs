using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.GetBaskets
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetBasketsQueryHandler : IRequestHandler<GetBasketsQuery, List<BasketDto>>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetBasketsQueryHandler(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<BasketDto>> Handle(GetBasketsQuery request, CancellationToken cancellationToken)
        {
            var baskets = await _basketRepository.FindAllAsync(cancellationToken);
            return baskets.MapToBasketDtoList(_mapper);
        }
    }
}