using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace Entities.PrivateSetters.EF.SqlServer.Domain.Entities
{
    public class Customer : Person
    {
        public string Status { get; private set; }
    }
}