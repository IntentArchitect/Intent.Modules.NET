using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Init.Application.Invoices
{
    public class OtherCustomerDto<T>
    {
        public OtherCustomerDto()
        {
        }

        public static OtherCustomerDto<T> Create()
        {
            return new OtherCustomerDto<T>
            {
            };
        }
    }
}