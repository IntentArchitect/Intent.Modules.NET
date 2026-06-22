using System;
using Intent.RoslynWeaver.Attributes;
using Wolverine.AzureFunctions.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandModels", Version = "1.0")]

namespace Wolverine.AzureFunctions.Application.Products.UpdateProductPrice
{
    public class UpdateProductPriceCommand : ICommand
    {
        public UpdateProductPriceCommand(Guid id, decimal price)
        {
            Id = id;
            Price = price;
        }

        public Guid Id { get; set; }
        public decimal Price { get; set; }
    }
}