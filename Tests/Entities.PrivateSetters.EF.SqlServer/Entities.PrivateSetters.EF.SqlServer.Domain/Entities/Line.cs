using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Domain.Entities
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Line
    {
        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Line()
        {
            Description = null!;
        }

        public Line(string description, int quantity)
        {
            Description = description;
            Quantity = quantity;
        }
        public Guid Id { get; private set; }

        public Guid InvoiceId { get; private set; }

        public string Description { get; private set; }

        public int Quantity { get; private set; }
    }
}