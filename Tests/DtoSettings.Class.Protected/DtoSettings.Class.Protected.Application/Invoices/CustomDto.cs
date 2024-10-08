using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Protected.Application.Invoices
{
    public class CustomDto
    {
        public CustomDto()
        {
        }

        protected CustomDto()
        {
        }

        public static CustomDto Create()
        {
            return new CustomDto();
        }
    }
}