using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.TimeConcepts
{
    public class TimeEntity
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public DateTime DateTime { get; set; }

        public DateTimeOffset DateTimeOffset { get; set; }

        public TimeSpan TimeSpan { get; set; }
    }
}