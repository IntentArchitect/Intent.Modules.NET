using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace CleanArchitecture.OnlyModeledDomainEvents.Domain.Entities
{
    public class Agg2
    {
        public Guid Id { get; set; }
    }
}