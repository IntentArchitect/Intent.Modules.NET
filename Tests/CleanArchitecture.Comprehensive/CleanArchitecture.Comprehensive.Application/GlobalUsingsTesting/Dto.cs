using CleanArchitecture.Comprehensive.Application.GlobalUsingsTesting.SubNamespace;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.GlobalUsingsTesting
{
    public class Dto
    {
        public Dto()
        {
            Field = null!;
        }

        public SubNamespace.Transaction Field { get; set; }

        public static Dto Create(SubNamespace.Transaction field)
        {
            return new Dto
            {
                Field = field
            };
        }
    }
}