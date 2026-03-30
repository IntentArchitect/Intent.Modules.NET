using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FluentValidationTest.Domain.Entities.ValidationScenarios.NumericConstraints
{
    public class NumericConstrainedEntity
    {
        public Guid Id { get; set; }

        public int Age { get; set; }

        public int Percentage { get; set; }

        public double Score { get; set; }

        public decimal Price { get; set; }

        public int? OptionalThreshold { get; set; }
    }
}