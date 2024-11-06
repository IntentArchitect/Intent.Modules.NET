using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Hierarchy
{
    public class Parent3 : Grandparent3
    {
        public Parent3()
        {
            ParentName = null!;
        }

        public int ParentId { get; set; }
        public string ParentName { get; set; }

        public static Parent3 Create(long grandparentId, string grandparentName, int parentId, string parentName)
        {
            return new Parent3
            {
                GrandparentId = grandparentId,
                GrandparentName = grandparentName,
                ParentId = parentId,
                ParentName = parentName
            };
        }
    }
}