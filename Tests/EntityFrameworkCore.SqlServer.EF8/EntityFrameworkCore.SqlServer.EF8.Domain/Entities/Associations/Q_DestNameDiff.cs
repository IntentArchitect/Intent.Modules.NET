using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Associations
{
    public class Q_DestNameDiff
    {
        public Q_DestNameDiff()
        {
            DestNameDiff = null!;
        }
        public Guid Id { get; set; }

        public virtual Q_DestNameDiffDependent DestNameDiff { get; set; }
    }
}