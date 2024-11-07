using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class FamilyComplex
    {
        public FamilyComplex(string childName,
            int parentId,
            string parentName,
            long grandParentId,
            long greatGrandParentId,
            string greatGrandParentName,
            int auntId,
            string auntName)
        {
            ChildName = childName;
            ParentId = parentId;
            ParentName = parentName;
            GrandParentId = grandParentId;
            GreatGrandParentId = greatGrandParentId;
            GreatGrandParentName = greatGrandParentName;
            AuntId = auntId;
            AuntName = auntName;
        }
    }
}