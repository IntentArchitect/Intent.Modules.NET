using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.DbContext.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.DbContext.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.GiftCards.CreateGiftCard
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateGiftCardCommandHandler : IRequestHandler<CreateGiftCardCommand, string>
    {
        private readonly IApplicationDbContext _dbContext;

        [IntentManaged(Mode.Merge)]
        public CreateGiftCardCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateGiftCardCommand request, CancellationToken cancellationToken)
        {
            var giftCard = new GiftCard
            {
                Id = request.Id,
                Value = request.Value,
                CustomerId = request.CustomerId
            };

            _dbContext.GiftCards.Add(giftCard);
            return giftCard.Id;
        }
    }
}