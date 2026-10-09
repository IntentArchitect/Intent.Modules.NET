using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.QueryModels", Version = "1.0")]

namespace CqrsSpike.Publisher.Application.AuditEntries.GetAuditEntries
{
    public class GetAuditEntriesQuery : IQuery<List<AuditEntryDto>>
    {
        public GetAuditEntriesQuery()
        {
        }
    }
}