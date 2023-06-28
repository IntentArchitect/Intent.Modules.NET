using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases
{
    public class UpdatePurchaseMoneyDto
    {
        public UpdatePurchaseMoneyDto()
        {
            Currency = null!;
        }

        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public static UpdatePurchaseMoneyDto Create(decimal amount, string currency)
        {
            return new UpdatePurchaseMoneyDto
            {
                Amount = amount,
                Currency = currency
            };
        }
    }
}