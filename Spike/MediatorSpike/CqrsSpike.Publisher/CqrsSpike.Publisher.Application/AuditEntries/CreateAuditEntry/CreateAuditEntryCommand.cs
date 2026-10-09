using CqrsSpike.Publisher.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CqrsSpike.Publisher.Application.AuditEntries.CreateAuditEntry
{
    public class CreateAuditEntryCommand : IRequest<Guid>, ICommand
    {
        public CreateAuditEntryCommand(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}