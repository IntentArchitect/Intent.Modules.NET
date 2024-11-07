using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Hierarchy
{
    public class Grandparent4 : GreatGrandparent4
    {
        public Grandparent4()
        {
        }

        public long GrandparentId { get; set; }

        public static Grandparent4 Create(long greatGrandparentId, string greatGrandparentName, long grandparentId)
        {
            return new Grandparent4
            {
                GreatGrandparentId = greatGrandparentId,
                GreatGrandparentName = greatGrandparentName,
                GrandparentId = grandparentId
            };
        }
    }
}