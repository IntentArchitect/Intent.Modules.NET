using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD
{
    public class CreateMoneyDto
    {
        public CreateMoneyDto()
        {
            Currency = null!;
        }

        public string Currency { get; set; }
        public decimal Amount { get; set; }

        public static CreateMoneyDto Create(string currency, decimal amount)
        {
            return new CreateMoneyDto
            {
                Currency = currency,
                Amount = amount
            };
        }
    }
}