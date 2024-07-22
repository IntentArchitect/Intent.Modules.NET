using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OpenApiImporterTest.Application.Accounts
{
    public class CreateAccountCommand
    {
        public CreateAccountCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateAccountCommand Create(string name)
        {
            return new CreateAccountCommand
            {
                Name = name
            };
        }
    }
}