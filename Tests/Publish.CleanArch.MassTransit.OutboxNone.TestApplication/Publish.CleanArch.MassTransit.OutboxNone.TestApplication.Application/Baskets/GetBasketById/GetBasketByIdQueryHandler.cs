using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Common.Exceptions;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.GetBasketById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetBasketByIdQueryHandler : IRequestHandler<GetBasketByIdQuery, BasketDto>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetBasketByIdQueryHandler(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<BasketDto> Handle(GetBasketByIdQuery request, CancellationToken cancellationToken)
        {
            var basket = await _basketRepository.FindByIdAsync(request.Id, cancellationToken);

            if (basket is null)
            {
                throw new NotFoundException($"Could not find Basket '{request.Id}'");
            }
            return basket.MapToBasketDto(_mapper);
        }
    }
}