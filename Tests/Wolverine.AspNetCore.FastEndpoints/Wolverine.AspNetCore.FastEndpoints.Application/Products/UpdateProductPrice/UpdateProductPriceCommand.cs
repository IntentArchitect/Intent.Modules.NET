using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.FastEndpoints.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandModels", Version = "1.0")]

namespace Wolverine.AspNetCore.FastEndpoints.Application.Products.UpdateProductPrice
{
    public class UpdateProductPriceCommand : ICommand
    {
        public UpdateProductPriceCommand(Guid id, decimal newPrice)
        {
            Id = id;
            NewPrice = newPrice;
        }

        public Guid Id { get; set; }
        public decimal NewPrice { get; set; }
    }
}