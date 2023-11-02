using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Validation
{
    public class ValidationDto
    {
        public ValidationDto()
        {
            Email = null!;
        }

        public string Email { get; set; }

        public static ValidationDto Create(string email)
        {
            return new ValidationDto
            {
                Email = email
            };
        }
    }
}