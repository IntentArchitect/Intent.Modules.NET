using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Hierarchy
{
    public class Parent4 : Grandparent4
    {
        public Parent4()
        {
        }

        public int ParentId { get; set; }

        public static Parent4 Create(long greatGrandparentId, string greatGrandparentName, long grandparentId, int parentId)
        {
            return new Parent4
            {
                GreatGrandparentId = greatGrandparentId,
                GreatGrandparentName = greatGrandparentName,
                GrandparentId = grandparentId,
                ParentId = parentId
            };
        }
    }
}