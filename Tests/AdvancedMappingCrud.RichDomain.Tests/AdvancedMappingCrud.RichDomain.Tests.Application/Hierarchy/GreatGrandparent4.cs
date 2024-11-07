using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Hierarchy
{
    public class GreatGrandparent4
    {
        public GreatGrandparent4()
        {
            GreatGrandparentName = null!;
        }

        public long GreatGrandparentId { get; set; }
        public string GreatGrandparentName { get; set; }

        public static GreatGrandparent4 Create(long greatGrandparentId, string greatGrandparentName)
        {
            return new GreatGrandparent4
            {
                GreatGrandparentId = greatGrandparentId,
                GreatGrandparentName = greatGrandparentName
            };
        }
    }
}