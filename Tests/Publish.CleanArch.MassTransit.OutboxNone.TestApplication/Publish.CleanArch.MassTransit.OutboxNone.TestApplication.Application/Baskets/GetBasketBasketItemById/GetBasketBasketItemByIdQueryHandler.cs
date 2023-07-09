using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Common.Exceptions;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Entities;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.GetBasketBasketItemById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetBasketBasketItemByIdQueryHandler : IRequestHandler<GetBasketBasketItemByIdQuery, BasketBasketItemDto>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetBasketBasketItemByIdQueryHandler(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<BasketBasketItemDto> Handle(
            GetBasketBasketItemByIdQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _basketRepository.FindByIdAsync(request.BasketId, cancellationToken);

            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(Basket)} of Id '{request.BasketId}' could not be found");
            }

            var element = aggregateRoot.BasketItems.FirstOrDefault(p => p.Id == request.Id);

            if (element is null)
            {
                throw new NotFoundException($"Could not find BasketItem '{request.Id}'");
            }
            return element.MapToBasketBasketItemDto(_mapper);
        }
    }
}