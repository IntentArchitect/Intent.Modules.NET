using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class Company
    {
        public Company(string name, IEnumerable<ContactDetailsVO> contactDetailsVOS)
        {
            Name = name;
            ContactDetailsVOS = new List<ContactDetailsVO>(contactDetailsVOS);
        }
    }
}