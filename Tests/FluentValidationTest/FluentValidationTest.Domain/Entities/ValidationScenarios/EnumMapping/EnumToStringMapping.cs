using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FluentValidationTest.Domain.Entities.ValidationScenarios.EnumMapping
{
    public class EnumToStringMapping
    {
        public EnumToStringMapping()
        {
            StatusText = null!;
            Notes = null!;
            ProcessText = null!;
        }

        public Guid Id { get; set; }

        public string StatusText { get; set; }

        public string Notes { get; set; }

        public string ProcessText { get; set; }
    }
}