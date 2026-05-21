using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.GiftCards.CreateGiftCard
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateGiftCardCommandHandler : IRequestHandler<CreateGiftCardCommand, string>
    {
        private readonly IGiftCardRepository _giftCardRepository;

        [IntentManaged(Mode.Merge)]
        public CreateGiftCardCommandHandler(IGiftCardRepository giftCardRepository)
        {
            _giftCardRepository = giftCardRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateGiftCardCommand request, CancellationToken cancellationToken)
        {
            var giftCard = new GiftCard
            {
                CardCode = request.CardCode,
                Value = request.Value,
                UserId = request.UserId
            };

            _giftCardRepository.Add(giftCard);
            return giftCard.CardCode;
        }
    }
}