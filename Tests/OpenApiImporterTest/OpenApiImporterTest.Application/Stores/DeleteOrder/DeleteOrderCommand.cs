using Intent.RoslynWeaver.Attributes;
using MediatR;
using OpenApiImporterTest.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace OpenApiImporterTest.Application.Stores.DeleteOrder
{
    public class DeleteOrderCommand : IRequest, ICommand
    {
        public DeleteOrderCommand(int orderId)
        {
            OrderId = orderId;
        }

        public int OrderId { get; set; }
    }
}