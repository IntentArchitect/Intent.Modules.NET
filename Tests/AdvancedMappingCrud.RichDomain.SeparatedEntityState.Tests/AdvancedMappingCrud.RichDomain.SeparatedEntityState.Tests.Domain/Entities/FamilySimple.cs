using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class FamilySimple
    {
        public FamilySimple(string childName, int parentId, string parentName, string grandparentName, long grandparentId)
        {
            ChildName = childName;
            ParentId = parentId;
            ParentName = parentName;
            GrandparentName = grandparentName;
            GrandparentId = grandparentId;
        }
    }
}