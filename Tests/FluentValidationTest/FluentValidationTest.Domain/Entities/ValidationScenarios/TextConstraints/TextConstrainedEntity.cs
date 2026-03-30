using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FluentValidationTest.Domain.Entities.ValidationScenarios.TextConstraints
{
    public class TextConstrainedEntity
    {
        public TextConstrainedEntity()
        {
            ShortCode = null!;
            DisplayName = null!;
            RequiredName = null!;
        }

        public Guid Id { get; set; }

        public string ShortCode { get; set; }

        public string DisplayName { get; set; }

        public string? Description { get; set; }

        public string RequiredName { get; set; }

        public string? OptionalNotes { get; set; }

        public string? NullButRequired { get; set; }

        public int DefaultIntButRequired { get; set; }
    }
}