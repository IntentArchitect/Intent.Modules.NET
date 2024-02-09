using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Parents
{
    public class GetParentByIdQuery
    {
        public Guid Id { get; set; }

        public static GetParentByIdQuery Create(Guid id)
        {
            return new GetParentByIdQuery
            {
                Id = id
            };
        }
    }
}