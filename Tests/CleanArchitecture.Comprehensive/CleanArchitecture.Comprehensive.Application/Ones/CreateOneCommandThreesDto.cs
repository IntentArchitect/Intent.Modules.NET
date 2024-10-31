using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Ones
{
    public class CreateOneCommandThreesDto
    {
        public CreateOneCommandThreesDto()
        {
            Finals = null!;
        }

        public List<CreateOneCommandFinalsDto> Finals { get; set; }
        public int ThreeId { get; set; }

        public static CreateOneCommandThreesDto Create(List<CreateOneCommandFinalsDto> finals, int threeId)
        {
            return new CreateOneCommandThreesDto
            {
                Finals = finals,
                ThreeId = threeId
            };
        }
    }
}