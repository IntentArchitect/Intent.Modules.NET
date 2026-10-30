using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandModels", Version = "1.0")]

namespace TestMediator.NoEventing.Application.Products.ChangeProductPrice
{
    public class ChangeProductPriceCommand : ICommand
    {
        public ChangeProductPriceCommand(Guid id, decimal newPrice)
        {
            Id = id;
            NewPrice = newPrice;
        }

        public Guid Id { get; set; }
        public decimal NewPrice { get; set; }
    }
}