using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.ExplicitETags
{
    public class UpdateExplicitETagCommand
    {
        public UpdateExplicitETagCommand()
        {
            Name = null!;
            Id = null!;
        }

        public string Name { get; set; }
        public string? ETag { get; set; }
        public string Id { get; set; }

        public static UpdateExplicitETagCommand Create(string name, string? eTag, string id)
        {
            return new UpdateExplicitETagCommand
            {
                Name = name,
                ETag = eTag,
                Id = id
            };
        }
    }
}