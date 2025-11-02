using CleanArchitecture.QueueStorage.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.QueueStorage.Application.Products.CreateProduct
{
    public class CreateProductCommand : IRequest<Guid>, ICommand
    {
        public CreateProductCommand(string name, int qty)
        {
            Name = name;
            Qty = qty;
        }

        public string Name { get; set; }
        public int Qty { get; set; }
    }
}