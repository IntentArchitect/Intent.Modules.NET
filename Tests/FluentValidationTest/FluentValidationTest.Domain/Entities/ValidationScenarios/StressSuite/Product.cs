using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FluentValidationTest.Domain.Entities.ValidationScenarios.StressSuite
{
    public class Product
    {
        public Product()
        {
            Code = null!;
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
    }
}