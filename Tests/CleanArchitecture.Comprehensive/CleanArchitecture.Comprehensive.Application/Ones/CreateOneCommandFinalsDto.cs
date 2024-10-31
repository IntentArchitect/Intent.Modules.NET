using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Ones
{
    public class CreateOneCommandFinalsDto
    {
        public CreateOneCommandFinalsDto()
        {
        }

        public int FourId { get; set; }

        public static CreateOneCommandFinalsDto Create(int fourId)
        {
            return new CreateOneCommandFinalsDto
            {
                FourId = fourId
            };
        }
    }
}