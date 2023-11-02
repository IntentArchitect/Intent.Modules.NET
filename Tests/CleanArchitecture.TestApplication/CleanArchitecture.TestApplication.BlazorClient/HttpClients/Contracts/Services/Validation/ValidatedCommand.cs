using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.Contracts.Services.Validation
{
    public class ValidatedCommand
    {
        public ValidatedCommand()
        {
            Field = null!;
            Email = null!;
        }

        [Required(ErrorMessage = "Field is required.")]
        public string Field { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        public string Email { get; set; }

        public static ValidatedCommand Create(string field, string email)
        {
            return new ValidatedCommand
            {
                Field = field,
                Email = email
            };
        }
    }
}