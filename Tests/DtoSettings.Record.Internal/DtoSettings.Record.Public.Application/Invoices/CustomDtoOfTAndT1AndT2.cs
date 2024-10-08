using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Invoices
{
    public record CustomDto<T, T1, T2>
    {
        public CustomDto()
        {
        }

        protected CustomDto()
        {
        }
    }
}