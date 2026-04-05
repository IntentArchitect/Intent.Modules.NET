using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.PrimaryKeyLookup
{
    public class ComponentTypeProperty
    {
        public ComponentTypeProperty()
        {
            PropertyName = null!;
        }

        public int PropertyId { get; set; }

        public string PropertyName { get; set; }

        public int ComponentPropertyGroupPropertyGroupId { get; set; }
    }
}