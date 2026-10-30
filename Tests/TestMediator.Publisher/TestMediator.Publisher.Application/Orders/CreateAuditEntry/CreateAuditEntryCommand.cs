using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandModels", Version = "1.0")]

namespace TestMediator.Publisher.Application.Orders.CreateAuditEntry
{
    public class CreateAuditEntryCommand : ICommand
    {
        public CreateAuditEntryCommand(Guid orderId, string message)
        {
            OrderId = orderId;
            Message = message;
        }

        public Guid OrderId { get; set; }
        public string Message { get; set; }
    }
}