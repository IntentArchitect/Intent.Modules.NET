using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Hierarchy
{
    public class Parent5 : Grandparent5
    {
        public Parent5()
        {
            ParentName = null!;
            Aunt = null!;
        }

        public int ParentId { get; set; }
        public string ParentName { get; set; }
        public Aunt5 Aunt { get; set; }

        public static Parent5 Create(
            long greatGrandParentId,
            string greatGrandParentName,
            long grandparentId,
            int parentId,
            string parentName,
            Aunt5 aunt)
        {
            return new Parent5
            {
                GreatGrandParentId = greatGrandParentId,
                GreatGrandParentName = greatGrandParentName,
                GrandparentId = grandparentId,
                ParentId = parentId,
                ParentName = parentName,
                Aunt = aunt
            };
        }
    }
}