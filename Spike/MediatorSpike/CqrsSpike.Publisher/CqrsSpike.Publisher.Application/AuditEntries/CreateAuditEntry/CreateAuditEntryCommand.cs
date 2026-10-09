using Mediator;

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