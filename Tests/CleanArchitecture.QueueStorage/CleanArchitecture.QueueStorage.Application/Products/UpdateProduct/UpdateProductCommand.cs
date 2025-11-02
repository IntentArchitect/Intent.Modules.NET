using CleanArchitecture.QueueStorage.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.QueueStorage.Application.Products.UpdateProduct
{
    public class UpdateProductCommand : IRequest, ICommand
    {
        public UpdateProductCommand(Guid id, string name, int qty)
        {
            Id = id;
            Name = name;
            Qty = qty;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }
    }
}