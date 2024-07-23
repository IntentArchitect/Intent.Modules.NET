using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Basics
{
    public class GetBasicByIdQuery
    {
        public GetBasicByIdQuery()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public static GetBasicByIdQuery Create(string id)
        {
            return new GetBasicByIdQuery
            {
                Id = id
            };
        }
    }
}