using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain
{
    public class ContactDetailsVO : ValueObject
    {
        public ContactDetailsVO(string cell, string email)
        {
            Cell = cell;
            Email = email;
        }

        protected ContactDetailsVO()
        {
            Cell = null!;
            Email = null!;
        }

        public string Cell { get; private set; }
        public string Email { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Cell;
            yield return Email;
        }
    }
}