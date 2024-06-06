using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD
{
    public class CreateAccountDto
    {
        public CreateAccountDto()
        {
            AccNumber = null!;
            Note = null!;
        }

        public string AccNumber { get; set; }
        public string Note { get; set; }

        public static CreateAccountDto Create(string accNumber, string note)
        {
            return new CreateAccountDto
            {
                AccNumber = accNumber,
                Note = note
            };
        }
    }
}