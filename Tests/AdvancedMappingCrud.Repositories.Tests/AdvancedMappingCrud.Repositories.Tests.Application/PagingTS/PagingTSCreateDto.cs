using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.PagingTS
{
    public class PagingTSCreateDto
    {
        public PagingTSCreateDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static PagingTSCreateDto Create(string name)
        {
            return new PagingTSCreateDto
            {
                Name = name
            };
        }
    }
}