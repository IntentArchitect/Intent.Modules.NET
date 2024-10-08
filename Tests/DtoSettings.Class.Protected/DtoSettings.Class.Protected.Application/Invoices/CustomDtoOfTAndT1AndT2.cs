using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Protected.Application.Invoices
{
    public class CustomDto<T, T1, T2>
    {
        public CustomDto()
        {
        }

        protected CustomDto()
        {
        }

        public static CustomDto<T, T1, T2> Create()
        {
            return new CustomDto<T, T1, T2>();
        }
    }
}