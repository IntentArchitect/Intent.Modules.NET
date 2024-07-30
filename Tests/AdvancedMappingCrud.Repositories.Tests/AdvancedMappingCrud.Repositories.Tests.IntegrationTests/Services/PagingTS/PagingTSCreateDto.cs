using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.PagingTS
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