using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandModels", Version = "1.0")]

namespace TestMediator.Publisher.Application.Orders.MarkOrderShipped
{
    public class MarkOrderShippedCommand : ICommand
    {
        public MarkOrderShippedCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}