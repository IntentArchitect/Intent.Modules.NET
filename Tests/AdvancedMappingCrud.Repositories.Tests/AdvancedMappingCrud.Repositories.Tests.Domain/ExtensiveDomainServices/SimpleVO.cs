using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.ExtensiveDomainServices
{
    public class SimpleVO : ValueObject
    {
        protected SimpleVO()
        {
        }

        public SimpleVO(string value1, int value2)
        {
            Value1 = value1;
            Value2 = value2;
        }

        public string Value1 { get; private set; }
        public int Value2 { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Value1;
            yield return Value2;
        }
    }
}