using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases
{
    public class CreatePurchaseMoneyDto
    {
        public CreatePurchaseMoneyDto()
        {
            Currency = null!;
        }

        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public static CreatePurchaseMoneyDto Create(decimal amount, string currency)
        {
            return new CreatePurchaseMoneyDto
            {
                Amount = amount,
                Currency = currency
            };
        }
    }
}