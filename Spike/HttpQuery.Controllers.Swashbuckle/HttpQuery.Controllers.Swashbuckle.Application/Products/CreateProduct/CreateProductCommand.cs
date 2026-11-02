using HttpQuery.Controllers.Swashbuckle.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace HttpQuery.Controllers.Swashbuckle.Application.Products.CreateProduct
{
    public class CreateProductCommand : IRequest<Guid>, ICommand
    {
        public CreateProductCommand(string name, string category, decimal price)
        {
            Name = name;
            Category = category;
            Price = price;
        }

        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
    }
}