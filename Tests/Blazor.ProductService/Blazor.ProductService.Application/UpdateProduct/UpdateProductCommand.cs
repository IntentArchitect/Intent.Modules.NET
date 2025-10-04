using Blazor.ProductService.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Blazor.ProductService.Application.UpdateProduct
{
    public class UpdateProductCommand : IRequest, ICommand
    {
        public UpdateProductCommand(Guid id, string name, string @ref, int qty)
        {
            Id = id;
            Name = name;
            Ref = @ref;
            Qty = qty;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Ref { get; set; }
        public int Qty { get; set; }
    }
}