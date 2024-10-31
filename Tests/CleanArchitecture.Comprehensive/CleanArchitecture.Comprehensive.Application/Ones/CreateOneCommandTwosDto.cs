using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Ones
{
    public class CreateOneCommandTwosDto
    {
        public CreateOneCommandTwosDto()
        {
            Threes = null!;
        }

        public List<CreateOneCommandThreesDto> Threes { get; set; }
        public int TwoId { get; set; }

        public static CreateOneCommandTwosDto Create(List<CreateOneCommandThreesDto> threes, int twoId)
        {
            return new CreateOneCommandTwosDto
            {
                Threes = threes,
                TwoId = twoId
            };
        }
    }
}