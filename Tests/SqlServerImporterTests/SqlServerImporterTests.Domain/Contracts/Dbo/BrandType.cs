using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace SqlServerImporterTests.Domain.Contracts.Dbo
{
    public record BrandType
    {
        public BrandType(string name, bool isActive)
        {
            Name = name;
            IsActive = isActive;
        }

        public string Name { get; init; }
        public bool IsActive { get; init; }
    }
}