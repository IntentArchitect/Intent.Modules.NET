using System;
using Intent.RoslynWeaver.Attributes;

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public class InvoiceLogo : IInvoiceLogo
    {
        public string Url { get; set; }
    }
}