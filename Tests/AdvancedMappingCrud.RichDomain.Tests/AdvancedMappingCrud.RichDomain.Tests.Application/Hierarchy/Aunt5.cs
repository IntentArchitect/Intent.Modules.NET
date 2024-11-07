using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Hierarchy
{
    public class Aunt5
    {
        public Aunt5()
        {
            AuntName = null!;
        }

        public int AuntId { get; set; }
        public string AuntName { get; set; }

        public static Aunt5 Create(int auntId, string auntName)
        {
            return new Aunt5
            {
                AuntId = auntId,
                AuntName = auntName
            };
        }
    }
}