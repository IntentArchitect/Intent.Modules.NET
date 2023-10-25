using System;
using Intent.RoslynWeaver.Attributes;

namespace CosmosDB.PrivateSetters.Domain.Entities
{
    public class InvoiceLogo
    {
        public string Url { get; private set; }
    }
}