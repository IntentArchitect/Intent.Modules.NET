using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Hierarchy
{
    public class Child3 : Parent3
    {
        public Child3()
        {
            ChildName = null!;
        }

        public string ChildName { get; set; }

        public static Child3 Create(
            long grandparentId,
            string grandparentName,
            int parentId,
            string parentName,
            string childName)
        {
            return new Child3
            {
                GrandparentId = grandparentId,
                GrandparentName = grandparentName,
                ParentId = parentId,
                ParentName = parentName,
                ChildName = childName
            };
        }
    }
}