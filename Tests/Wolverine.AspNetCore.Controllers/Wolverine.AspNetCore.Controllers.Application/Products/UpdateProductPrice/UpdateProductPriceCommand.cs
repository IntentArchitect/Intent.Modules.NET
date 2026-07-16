using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandModels", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.UpdateProductPrice
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