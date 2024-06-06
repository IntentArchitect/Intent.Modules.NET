using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.AspNetCore.Identity.AccountController
{
    public class ConfirmEmailDto
    {
        public ConfirmEmailDto()
        {
            UserId = null!;
            Code = null!;
        }

        [Required(ErrorMessage = "User id is required.")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Code is required.")]
        public string Code { get; set; }

        public static ConfirmEmailDto Create(string userId, string code)
        {
            return new ConfirmEmailDto
            {
                UserId = userId,
                Code = code
            };
        }
    }
}