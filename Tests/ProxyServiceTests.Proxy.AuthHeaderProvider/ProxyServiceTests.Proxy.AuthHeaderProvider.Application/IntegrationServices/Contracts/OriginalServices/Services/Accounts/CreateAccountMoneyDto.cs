using Intent.RoslynWeaver.Attributes;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices.Contracts.OriginalServices.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices.Contracts.OriginalServices.Services.Accounts
{
    public class CreateAccountMoneyDto
    {
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