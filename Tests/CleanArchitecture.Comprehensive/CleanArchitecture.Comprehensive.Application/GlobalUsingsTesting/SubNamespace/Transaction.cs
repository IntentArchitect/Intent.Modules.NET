using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.GlobalUsingsTesting.SubNamespace
{
    public class Transaction
    {
        public Transaction()
        {
        }

        public static Transaction Create()
        {
            return new Transaction
            {
            };
        }
    }
}