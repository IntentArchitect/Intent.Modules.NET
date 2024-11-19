using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.AnemicChild
{
    public class AnemicChild
    {
        public AnemicChild()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
        }
        public Guid Id { get; set; }

        public Guid ParentWithAnemicChildId { get; set; }

        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string City { get; set; }
    }
}