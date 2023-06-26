using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Entities;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.GetBasketBasketItems
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetBasketBasketItemsQueryHandler : IRequestHandler<GetBasketBasketItemsQuery, List<BasketBasketItemDto>>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetBasketBasketItemsQueryHandler(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<BasketBasketItemDto>> Handle(
            GetBasketBasketItemsQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _basketRepository.FindByIdAsync(request.BasketId, cancellationToken);
            if (aggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(Basket)} of Id '{request.BasketId}' could not be found");
            }
            return aggregateRoot.BasketItems.MapToBasketBasketItemDtoList(_mapper);
        }
    }
}