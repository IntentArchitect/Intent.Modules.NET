using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Hierarchy
{
    public class GreatGrandparent5
    {
        public GreatGrandparent5()
        {
            GreatGrandParentName = null!;
        }

        public long GreatGrandParentId { get; set; }
        public string GreatGrandParentName { get; set; }

        public static GreatGrandparent5 Create(long greatGrandParentId, string greatGrandParentName)
        {
            return new GreatGrandparent5
            {
                GreatGrandParentId = greatGrandParentId,
                GreatGrandParentName = greatGrandParentName
            };
        }
    }
}