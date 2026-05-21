using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.GiftCards.DeleteGiftCard
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteGiftCardCommandHandler : IRequestHandler<DeleteGiftCardCommand>
    {
        private readonly IGiftCardRepository _giftCardRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteGiftCardCommandHandler(IGiftCardRepository giftCardRepository)
        {
            _giftCardRepository = giftCardRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteGiftCardCommand request, CancellationToken cancellationToken)
        {
            var giftCard = await _giftCardRepository.FindByIdAsync(request.CardCode, cancellationToken);
            if (giftCard is null)
            {
                throw new NotFoundException($"Could not find GiftCard '{request.CardCode}'");
            }


            _giftCardRepository.Remove(giftCard);
        }
    }
}