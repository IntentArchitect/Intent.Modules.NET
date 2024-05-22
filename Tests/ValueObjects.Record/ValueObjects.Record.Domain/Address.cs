using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace ValueObjects.Record.Domain
{
    public record Address(string Line1, string Line2, string City, string Country, AddressType AddressType);
}