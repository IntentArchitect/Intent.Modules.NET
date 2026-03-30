using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FluentValidationTest.Domain.Entities.ValidationScenarios.Nullability
{
    public class NullabilityConstrainedEntity
    {
        public NullabilityConstrainedEntity()
        {
            RequiredString = null!;
        }

        public Guid Id { get; set; }

        public string RequiredString { get; set; }

        public string? OptionalString { get; set; }

        public int RequiredInt { get; set; }

        public int? OptionalInt { get; set; }

        public Guid RequiredGuidValue { get; set; }

        public Guid? OptionalGuidValue { get; set; }

        public DateTime RequiredDateValue { get; set; }

        public DateTime? OptionalDateValue { get; set; }
    }
}