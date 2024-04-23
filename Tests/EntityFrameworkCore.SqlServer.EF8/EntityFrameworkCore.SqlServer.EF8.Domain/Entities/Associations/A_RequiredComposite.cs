using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Associations
{
    public class A_RequiredComposite
    {
        public Guid Id { get; set; }

        public string RequiredCompAttr { get; set; }

        public virtual A_OptionalDependent? A_OptionalDependent { get; set; }
    }
}