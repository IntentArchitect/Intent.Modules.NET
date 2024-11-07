using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class FamilyComplexSkipped
    {
        public FamilyComplexSkipped(string childName,
            int parentId,
            long grandparentId,
            long greatGrandparentId,
            string greatGrandparentName)
        {
            ChildName = childName;
            ParentId = parentId;
            GrandparentId = grandparentId;
            GreatGrandparentId = greatGrandparentId;
            GreatGrandparentName = greatGrandparentName;
        }
    }
}