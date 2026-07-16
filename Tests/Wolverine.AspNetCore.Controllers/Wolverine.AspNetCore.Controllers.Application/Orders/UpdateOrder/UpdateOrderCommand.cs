using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandModels", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.UpdateOrder
{
    public class UpdateOrderCommand : ICommand
    {
        public UpdateOrderCommand(Guid id, string orderNumber, string customerName, string? notes)
        {
            Id = id;
            OrderNumber = orderNumber;
            CustomerName = customerName;
            Notes = notes;
        }

        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string? Notes { get; set; }
    }
}