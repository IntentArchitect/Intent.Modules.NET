using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Ones
{
    public class UpdateOneTwoDto
    {
        public UpdateOneTwoDto()
        {
            TwoName = null!;
        }
        public string TwoName { get; set; }

        public static UpdateOneTwoDto Create(string twoName)
        {
            return new UpdateOneTwoDto
            {
                TwoName = twoName
            };
        }
    }
}