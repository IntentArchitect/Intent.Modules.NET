using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Hierarchy
{
    public class Child4 : Parent4
    {
        public Child4()
        {
            ChildName = null!;
        }

        public string ChildName { get; set; }

        public static Child4 Create(
            long greatGrandparentId,
            string greatGrandparentName,
            long grandparentId,
            int parentId,
            string childName)
        {
            return new Child4
            {
                GreatGrandparentId = greatGrandparentId,
                GreatGrandparentName = greatGrandparentName,
                GrandparentId = grandparentId,
                ParentId = parentId,
                ChildName = childName
            };
        }
    }
}