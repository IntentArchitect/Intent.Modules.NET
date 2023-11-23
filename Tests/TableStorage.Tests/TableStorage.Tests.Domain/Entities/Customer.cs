using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace TableStorage.Tests.Domain.Entities
{
    public class Customer
    {
        public string Name { get; set; }
    }
}