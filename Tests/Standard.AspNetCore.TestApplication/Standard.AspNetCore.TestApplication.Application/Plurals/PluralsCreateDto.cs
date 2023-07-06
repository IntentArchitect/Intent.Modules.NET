using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Plurals
{
    public class PluralsCreateDto
    {
        public PluralsCreateDto()
        {
        }

        public static PluralsCreateDto Create()
        {
            return new PluralsCreateDto
            {
            };
        }
    }
}