using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities
{
    public class UserDefaultAddress
    {
        public UserDefaultAddress()
        {
            Line1 = null!;
            Line2 = null!;
        }
        public Guid Id { get; set; }

        public string Line1 { get; set; }

        public string Line2 { get; set; }
    }
}