using System;
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
        }
        public string Email { get; set; }

        public Guid QuoteId { get; set; }

        public virtual Quote Quote { get; set; }
    }
}