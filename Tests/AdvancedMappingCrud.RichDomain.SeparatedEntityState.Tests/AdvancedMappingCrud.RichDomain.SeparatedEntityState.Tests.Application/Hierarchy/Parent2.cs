using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Hierarchy
{
    public class Parent2
    {
        public Parent2()
        {
        }

        public int ParentAge { get; set; }

        public static Parent2 Create(int parentAge)
        {
            return new Parent2
            {
                ParentAge = parentAge
            };
        }
    }
}