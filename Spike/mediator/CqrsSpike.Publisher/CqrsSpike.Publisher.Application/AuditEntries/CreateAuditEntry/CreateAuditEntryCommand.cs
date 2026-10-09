using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandModels", Version = "1.0")]

namespace CqrsSpike.Publisher.Application.AuditEntries.CreateAuditEntry
{
    public class CreateAuditEntryCommand : ICommand<Guid>
    {
        public CreateAuditEntryCommand(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}