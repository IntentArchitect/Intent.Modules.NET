using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace EntityFrameworkCore.MaintainColumnOrder.Tests.Domain
{
    public class InLineColumns : ValueObject
    {
        protected InLineColumns()
        {
        }

        public InLineColumns(string col1, string col2)
        {
            Col1 = col1;
            Col2 = col2;
        }

        public string Col1 { get; private set; }
        public string Col2 { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Col1;
            yield return Col2;
        }
    }
}