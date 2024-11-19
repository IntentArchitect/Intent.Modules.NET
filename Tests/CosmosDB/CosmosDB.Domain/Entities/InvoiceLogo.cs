using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.Domain.Entities
{
    public class InvoiceLogo
    {
        public InvoiceLogo()
        {
            Url = null!;
        }
        public string Url { get; set; }
    }
}