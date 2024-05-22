using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.NestingParents
{
    public class ManualChildChildDto
    {
        public ManualChildChildDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static ManualChildChildDto Create(string name)
        {
            return new ManualChildChildDto
            {
                Name = name
            };
        }
    }
}