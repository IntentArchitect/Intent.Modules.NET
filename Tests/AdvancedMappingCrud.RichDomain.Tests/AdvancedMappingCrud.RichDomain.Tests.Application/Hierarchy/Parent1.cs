using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Hierarchy
{
    public class Parent1
    {
        public Parent1()
        {
            ParentName = null!;
        }

        public string ParentName { get; set; }

        public static Parent1 Create(string parentName)
        {
            return new Parent1
            {
                ParentName = parentName
            };
        }
    }
}