using Intent.RoslynWeaver.Attributes;
using ProxyServiceTests.OriginalServices.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ProxyServiceTests.OriginalServices.Application.Accounts
{
    public class UpdateAccountMoneyDto
    {
        public UpdateAccountMoneyDto()
        {
        }

        public decimal Amount { get; set; }
        public Currency Currency { get; set; }

        public static UpdateAccountMoneyDto Create(decimal amount, Currency currency)
        {
            return new UpdateAccountMoneyDto
            {
                Amount = amount,
                Currency = currency
            };
        }
    }
}