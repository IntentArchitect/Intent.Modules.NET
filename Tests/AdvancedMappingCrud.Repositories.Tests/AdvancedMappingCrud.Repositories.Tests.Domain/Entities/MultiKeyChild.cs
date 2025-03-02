using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities
{
    public class MultiKeyChild
    {
        public MultiKeyChild()
        {
            RefNo = null!;
            ChildName = null!;
        }

        public Guid Id { get; set; }

        public string RefNo { get; set; }

        public string ChildName { get; set; }

        public Guid MultiKeyParentId { get; set; }
    }
}