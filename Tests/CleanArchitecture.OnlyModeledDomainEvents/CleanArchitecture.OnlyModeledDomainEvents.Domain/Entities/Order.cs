using System;
using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.OnlyModeledDomainEvents.Domain.Entities
{
    public class Order
    {
        private string? _id;

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }
    }
}