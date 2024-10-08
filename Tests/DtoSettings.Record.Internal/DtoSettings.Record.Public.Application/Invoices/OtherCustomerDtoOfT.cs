using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Invoices
{
    public record OtherCustomerDto<T>
    {
        public OtherCustomerDto()
        {
        }

        protected OtherCustomerDto()
        {
        }
    }
}