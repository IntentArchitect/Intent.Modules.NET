using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Protected.Application.Invoices
{
    public class NewDto
    {
        public NewDto()
        {
        }

        public static NewDto Create()
        {
            return new NewDto();
        }
    }
}