using Intent.RoslynWeaver.Attributes;
using ProxyServiceTests.Proxy.TMS.Application.IntegrationServices.Contracts.OriginalServices.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ProxyServiceTests.Proxy.TMS.Application.AccountsServices
{
    public class CreateAccountMoneyDto
    {
        public CreateAccountMoneyDto()
        {
        }

        public decimal Amount { get; set; }
        public Currency Currency { get; set; }

        public static CreateAccountMoneyDto Create(decimal amount, Currency currency)
        {
            return new CreateAccountMoneyDto
            {
                Amount = amount,
                Currency = currency
            };
        }
    }
}