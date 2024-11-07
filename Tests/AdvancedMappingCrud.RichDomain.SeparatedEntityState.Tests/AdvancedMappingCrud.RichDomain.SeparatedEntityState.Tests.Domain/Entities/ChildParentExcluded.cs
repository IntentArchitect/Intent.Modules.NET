using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class ChildParentExcluded
    {
        public ChildParentExcluded(string childName, int parentAge)
        {
            ChildName = childName;
            ParentAge = parentAge;
        }
    }
}