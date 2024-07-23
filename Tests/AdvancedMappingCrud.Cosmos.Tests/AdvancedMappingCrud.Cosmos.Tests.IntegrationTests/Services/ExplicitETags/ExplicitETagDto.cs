using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.ExplicitETags
{
    public class ExplicitETagDto
    {
        public ExplicitETagDto()
        {
            Id = null!;
            Name = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string? ETag { get; set; }

        public static ExplicitETagDto Create(string id, string name, string? eTag)
        {
            return new ExplicitETagDto
            {
                Id = id,
                Name = name,
                ETag = eTag
            };
        }
    }
}