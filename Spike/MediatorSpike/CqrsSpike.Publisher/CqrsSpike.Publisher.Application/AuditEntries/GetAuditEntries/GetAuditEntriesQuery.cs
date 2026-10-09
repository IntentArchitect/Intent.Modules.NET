using CqrsSpike.Publisher.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CqrsSpike.Publisher.Application.AuditEntries.GetAuditEntries
{
    public class GetAuditEntriesQuery : IRequest<List<AuditEntryDto>>, IQuery
    {
        public GetAuditEntriesQuery()
        {
        }
    }
}