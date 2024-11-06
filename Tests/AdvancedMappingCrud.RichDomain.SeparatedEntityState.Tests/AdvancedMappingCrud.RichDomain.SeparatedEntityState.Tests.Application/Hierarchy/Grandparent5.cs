using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Hierarchy
{
    public class Grandparent5 : GreatGrandparent5
    {
        public Grandparent5()
        {
        }

        public long GrandparentId { get; set; }

        public static Grandparent5 Create(long greatGrandParentId, string greatGrandParentName, long grandparentId)
        {
            return new Grandparent5
            {
                GreatGrandParentId = greatGrandParentId,
                GreatGrandParentName = greatGrandParentName,
                GrandparentId = grandparentId
            };
        }
    }
}