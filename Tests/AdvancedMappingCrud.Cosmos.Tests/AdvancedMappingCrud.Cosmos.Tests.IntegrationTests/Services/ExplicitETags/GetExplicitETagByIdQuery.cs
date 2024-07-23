using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.ExplicitETags
{
    public class GetExplicitETagByIdQuery
    {
        public GetExplicitETagByIdQuery()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public static GetExplicitETagByIdQuery Create(string id)
        {
            return new GetExplicitETagByIdQuery
            {
                Id = id
            };
        }
    }
}