using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FluentValidationTest.Domain.Entities.ValidationScenarios.PatternConstraints
{
    public class PatternConstrainedEntity
    {
        public PatternConstrainedEntity()
        {
            EmailAddress = null!;
            Slug = null!;
            ReferenceNumber = null!;
            Base64 = null!;
        }

        public Guid Id { get; set; }

        public string EmailAddress { get; set; }

        public string? WebsiteUrl { get; set; }

        public string Slug { get; set; }

        public string ReferenceNumber { get; set; }

        public string? OptionalPatternText { get; set; }

        public string Base64 { get; set; }
    }
}