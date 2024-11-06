using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Hierarchy
{
    public class Grandparent3
    {
        public Grandparent3()
        {
            GrandparentName = null!;
        }

        public long GrandparentId { get; set; }
        public string GrandparentName { get; set; }

        public static Grandparent3 Create(long grandparentId, string grandparentName)
        {
            return new Grandparent3
            {
                GrandparentId = grandparentId,
                GrandparentName = grandparentName
            };
        }
    }
}