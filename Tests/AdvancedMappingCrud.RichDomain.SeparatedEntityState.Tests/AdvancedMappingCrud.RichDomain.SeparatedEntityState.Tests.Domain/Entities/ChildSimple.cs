using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class ChildSimple
    {
        public ChildSimple(string childName, string parentName)
        {
            ChildName = childName;
            ParentName = parentName;
        }
    }
}