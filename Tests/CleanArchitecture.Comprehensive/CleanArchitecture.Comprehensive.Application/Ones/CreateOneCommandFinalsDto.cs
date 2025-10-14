using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Ones
{
    public class CreateOneCommandFinalsDto
    {
        public CreateOneCommandFinalsDto()
        {
            Attribute = null!;
        }

        public int FourId { get; set; }
        public string Attribute { get; set; }

        public static CreateOneCommandFinalsDto Create(int fourId, string attribute)
        {
            return new CreateOneCommandFinalsDto
            {
                FourId = fourId,
                Attribute = attribute
            };
        }
    }
}