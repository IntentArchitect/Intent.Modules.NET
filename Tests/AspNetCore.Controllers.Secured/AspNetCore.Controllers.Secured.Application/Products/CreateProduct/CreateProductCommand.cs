using System;
using AspNetCore.Controllers.Secured.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AspNetCore.Controllers.Secured.Application.Products.CreateProduct
{
    public class CreateProductCommand : IRequest<Guid>, ICommand
    {
        public CreateProductCommand(string name, string description, decimal price)
        {
            Name = name;
            Description = description;
            Price = price;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}