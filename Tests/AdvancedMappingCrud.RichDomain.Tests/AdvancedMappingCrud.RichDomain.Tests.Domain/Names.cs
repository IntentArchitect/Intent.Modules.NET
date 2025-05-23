using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain
{
    public class Names : ValueObject
    {
        public Names(string first, string last)
        {
            First = first;
            Last = last;
        }

        protected Names()
        {
            First = null!;
            Last = null!;
        }

        public string First { get; private set; }
        public string Last { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return First;
            yield return Last;
        }
    }
}