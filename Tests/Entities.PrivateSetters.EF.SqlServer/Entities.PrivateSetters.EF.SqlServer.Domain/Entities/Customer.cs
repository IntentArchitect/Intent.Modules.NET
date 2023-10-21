using Intent.RoslynWeaver.Attributes;

namespace Entities.PrivateSetters.EF.SqlServer.Domain.Entities
{
    public class Customer : Person
    {
        public string Status { get; private set; }
    }
}