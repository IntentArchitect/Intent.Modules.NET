using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Protected.Application.Invoices
{
    public class NewDto<T>
    {
        public NewDto()
        {
        }

        public static NewDto<T> Create()
        {
            return new NewDto<T>();
        }
    }
}