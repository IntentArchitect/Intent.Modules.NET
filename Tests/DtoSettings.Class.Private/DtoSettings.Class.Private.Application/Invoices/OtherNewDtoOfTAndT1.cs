using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Private.Application.Invoices
{
    public class OtherNewDto<T, T1>
    {
        public OtherNewDto()
        {
        }

        public static OtherNewDto<T, T1> Create()
        {
            return new OtherNewDto<T, T1>();
        }
    }
}