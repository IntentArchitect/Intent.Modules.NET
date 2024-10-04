using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Clients
{
    public class GetClientByIdQuery
    {
        public Guid Id { get; set; }

        public static GetClientByIdQuery Create(Guid id)
        {
            return new GetClientByIdQuery
            {
                Id = id
            };
        }
    }
}