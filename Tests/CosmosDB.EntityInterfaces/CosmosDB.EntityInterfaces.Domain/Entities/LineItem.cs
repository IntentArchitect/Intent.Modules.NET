using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public class LineItem : ILineItem
    {
        private string? _id;

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Description { get; set; }

        public int Quantity { get; set; }
    }
}