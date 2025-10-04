using Blazor.ProductService.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Blazor.ProductService.Application.CreateProduct
{
    public class CreateProductCommand : IRequest<Guid>, ICommand
    {
        public CreateProductCommand(string name, string @ref, int qty)
        {
            Name = name;
            Ref = @ref;
            Qty = qty;
        }

        public string Name { get; set; }
        public string Ref { get; set; }
        public int Qty { get; set; }
    }
}