using System;
using Intent.RoslynWeaver.Attributes;
using Wolverine.AzureFunctions.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandModels", Version = "1.0")]

namespace Wolverine.AzureFunctions.Application.Products.CreateProduct
{
    public class CreateProductCommand : ICommand
    {
        public CreateProductCommand(string name, decimal price, bool isActive)
        {
            Name = name;
            Price = price;
            IsActive = isActive;
        }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }
}