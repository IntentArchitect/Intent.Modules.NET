using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.SingleFiles.Domain.Entities
{
    public class EfLine
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid EfInvoicesId { get; set; }
    }
}