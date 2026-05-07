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

        /// <summary>
        /// Tests min exclusive (>), max inclusive (<=). Value must be > 0 and <= 100.
        /// </summary>
        public float ExclusiveMinInclusiveMaxFloat { get; set; }

        /// <summary>
        /// Tests min inclusive (>=), max exclusive (<). Value must be >= 10 and < 50.
        /// </summary>
        public double InclusiveMinExclusiveMaxDouble { get; set; }

        /// <summary>
        /// Tests both boundaries exclusive (> and <). Value must be > 0.01 and < 999.99.
        /// </summary>
        public decimal ExclusiveMinExclusiveMaxDecimal { get; set; }

        /// <summary>
        /// Tests only min boundary defined as exclusive (>). Value must be > -10.5.
        /// </summary>
        public float OnlyMinExclusiveFloat { get; set; }

        /// <summary>
        /// Tests only max boundary defined as exclusive (<). Value must be < 1000.0.
        /// </summary>
        public double OnlyMaxExclusiveDouble { get; set; }

        /// <summary>
        /// Tests both boundaries inclusive (>= and <=). Value must be >= 5.5 and <= 99.9.
        /// </summary>
        public float InclusiveMinInclusiveMaxFloat { get; set; }

        /// <summary>
        /// Tests negative number range with mixed boundaries. Value must be >= -100.00 and < -0.01.
        /// </summary>
        public decimal NegativeRangeDecimal { get; set; }

        /// <summary>
        /// Tests very narrow range with exclusive boundaries. Value must be > 0.001 and < 0.999.
        /// </summary>
        public double NarrowRangeDouble { get; set; }
    }
}