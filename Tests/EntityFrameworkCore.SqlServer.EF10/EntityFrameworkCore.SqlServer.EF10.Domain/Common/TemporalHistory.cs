using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.TemporalTables.TemporalHistory", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Domain.Common
{
    public class TemporalHistory<TEntity>
    {
        public TemporalHistory(TEntity entity, DateTime validFrom, DateTime validTo)
        {
            Entity = entity;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }

        public TEntity Entity { get; private set; }
        public DateTime ValidFrom { get; private set; }
        public DateTime ValidTo { get; private set; }
    }

    public record TemporalHistoryQueryOptions(TemporalHistoryQueryType? QueryType = TemporalHistoryQueryType.All,
        DateTime? DateFrom = null,
        DateTime? DateTo = null);


    public enum TemporalHistoryQueryType
    {
        All,

        AsOf,

        Between,

        ContainedIn,

        FromTo
    }
}