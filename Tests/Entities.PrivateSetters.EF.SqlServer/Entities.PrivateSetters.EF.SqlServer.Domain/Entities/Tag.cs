using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

namespace Entities.PrivateSetters.EF.SqlServer.Domain.Entities
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Tag
    {
        public Tag(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Tag()
        {
            Name = null!;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        protected virtual ICollection<Invoice> Invoices { get; set; }
    }
}