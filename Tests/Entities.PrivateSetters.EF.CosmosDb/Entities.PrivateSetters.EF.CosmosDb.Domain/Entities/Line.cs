using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace Entities.PrivateSetters.EF.CosmosDb.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Line
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public Line(string description, int quantity)
        {
            Description = description;
            Quantity = quantity;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected Line()
        {
            Description = null!;
        }

        public Guid Id { get; set; }

        public Guid InvoiceId { get; private set; }

        public string Description { get; private set; }

        public int Quantity { get; private set; }
    }
}