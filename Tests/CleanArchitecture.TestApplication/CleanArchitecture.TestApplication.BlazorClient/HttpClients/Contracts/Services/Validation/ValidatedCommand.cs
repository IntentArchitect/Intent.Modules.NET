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
            CascaseTest = null!;
        }

        [Required(ErrorMessage = "Field is required.")]
        public string Field { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Cascase test is required.")]
        [MinLength(1, ErrorMessage = "Cascase test must be 1 or more characters.")]
        public string CascaseTest { get; set; }

        public static ValidatedCommand Create(string field, string email, string cascaseTest)
        {
            return new ValidatedCommand
            {
                Field = field,
                Email = email,
                CascaseTest = cascaseTest
            };
        }
    }
}