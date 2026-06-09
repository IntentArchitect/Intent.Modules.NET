using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.GiftCards.CreateGiftCard
{
    public class CreateGiftCardCommand : IRequest<string>, ICommand
    {
        public CreateGiftCardCommand(string cardCode, decimal value, Guid? userId)
        {
            CardCode = cardCode;
            Value = value;
            UserId = userId;
        }

        public string CardCode { get; set; }
        public decimal Value { get; set; }
        public Guid? UserId { get; set; }
    }
}