using System;
using System.ComponentModel.DataAnnotations;
using AdvancedMappingCrud.DbContext.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.GiftCards.CreateGiftCard
{
    public class CreateGiftCardCommand : IRequest<string>, ICommand
    {
        public CreateGiftCardCommand(string id, decimal value, Guid? customerId)
        {
            Id = id;
            Value = value;
            CustomerId = customerId;
        }

        public string Id { get; set; }
        public decimal Value { get; set; }
        public Guid? CustomerId { get; set; }
    }
}