using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Associations
{
    public class C_RequiredComposite
    {
        public Guid Id { get; set; }

        public string RequiredCompAttr { get; set; }

        public virtual ICollection<C_MultipleDependent> C_MultipleDependents { get; set; } = new List<C_MultipleDependent>();
    }
}