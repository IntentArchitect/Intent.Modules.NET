using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.GiftCards.GetGiftCardById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetGiftCardByIdQueryHandler : IRequestHandler<GetGiftCardByIdQuery, GiftCardDto>
    {
        private readonly IGiftCardRepository _giftCardRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetGiftCardByIdQueryHandler(IGiftCardRepository giftCardRepository, IMapper mapper)
        {
            _giftCardRepository = giftCardRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<GiftCardDto> Handle(GetGiftCardByIdQuery request, CancellationToken cancellationToken)
        {
            var giftCard = await _giftCardRepository.FindByIdAsync(request.CardCode, cancellationToken);
            if (giftCard is null)
            {
                throw new NotFoundException($"Could not find GiftCard '{request.CardCode}'");
            }
            return giftCard.MapToGiftCardDto(_mapper);
        }
    }
}