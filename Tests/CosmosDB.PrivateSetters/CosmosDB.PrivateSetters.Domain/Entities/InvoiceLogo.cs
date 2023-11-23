using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace CosmosDB.PrivateSetters.Domain.Entities
{
    public class InvoiceLogo
    {
        public string Url { get; private set; }
    }
}