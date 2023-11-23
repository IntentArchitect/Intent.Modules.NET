using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public class InvoiceLogo : IInvoiceLogo
    {
        public string Url { get; set; }
    }
}