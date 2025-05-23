using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module2.Application.Contracts.Accounts
{
    public class AccountDto
    {
        public AccountDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static AccountDto Create(Guid id, string name)
        {
            return new AccountDto
            {
                Id = id,
                Name = name
            };
        }
    }
}