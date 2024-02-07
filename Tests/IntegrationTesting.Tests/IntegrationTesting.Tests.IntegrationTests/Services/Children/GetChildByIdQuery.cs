using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Children
{
    public class GetChildByIdQuery
    {
        public Guid Id { get; set; }

        public static GetChildByIdQuery Create(Guid id)
        {
            return new GetChildByIdQuery
            {
                Id = id
            };
        }
    }
}