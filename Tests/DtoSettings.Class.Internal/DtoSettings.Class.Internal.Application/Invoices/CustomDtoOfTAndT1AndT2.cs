using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Internal.Application.Invoices
{
    public class CustomDto<T, T1, T2>
    {
        public CustomDto()
        {
        }

        protected CustomDto()
        {
        }
    }
}