using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Parents
{
    public class GetParentByIdQuery
    {
        public GetParentByIdQuery()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public static GetParentByIdQuery Create(string id)
        {
            return new GetParentByIdQuery
            {
                Id = id
            };
        }
    }
}