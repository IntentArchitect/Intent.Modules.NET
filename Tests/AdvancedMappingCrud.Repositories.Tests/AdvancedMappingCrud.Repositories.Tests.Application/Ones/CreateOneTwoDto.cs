using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Ones
{
    public class CreateOneTwoDto
    {
        public CreateOneTwoDto()
        {
            TwoName = null!;
        }

        public string TwoName { get; set; }

        public static CreateOneTwoDto Create(string twoName)
        {
            return new CreateOneTwoDto
            {
                TwoName = twoName
            };
        }
    }
}