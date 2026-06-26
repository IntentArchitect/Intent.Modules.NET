using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandModels", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.Products.ChangeProductProduct
{
    public class ChangeProductProductCommand : ICommand
    {
        public ChangeProductProductCommand(Guid id, string name, decimal price)
        {
            Id = id;
            Name = name;
            Price = price;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}