using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.PagingTS
{
    public class PagingTSUpdateDto
    {
        public PagingTSUpdateDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static PagingTSUpdateDto Create(Guid id, string name)
        {
            return new PagingTSUpdateDto
            {
                Id = id,
                Name = name
            };
        }
    }
}