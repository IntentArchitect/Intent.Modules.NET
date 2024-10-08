using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Internal.Application.Invoices
{
    public class OtherCustomerDto<T>
    {
        public OtherCustomerDto()
        {
        }

        protected OtherCustomerDto()
        {
        }
    }
}