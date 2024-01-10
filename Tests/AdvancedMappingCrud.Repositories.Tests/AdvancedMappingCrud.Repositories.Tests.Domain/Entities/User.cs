using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities
{
    public class User : Person
    {
        public User(string name, string surname, string email, Guid quoteId)
        {
            Name = name;
            Surname = surname;
            Email = email;
            QuoteId = quoteId;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected User()
        {
            Email = null!;
            Quote = null!;
            DefaultDeliveryAddress = null!;
        }
        public string Email { get; set; }

        public Guid QuoteId { get; set; }

        public virtual Quote Quote { get; set; }

        public virtual ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();

        public virtual UserDefaultAddress DefaultDeliveryAddress { get; set; }

        public virtual UserDefaultAddress? DefaultBillingAddress { get; set; }

        public void AddAddress(IEnumerable<UserAddress> addresses)
        {
            Addresses.Clear();

            foreach (var item in addresses)
            {
                Addresses.Add(item);
            }
        }
    }
}