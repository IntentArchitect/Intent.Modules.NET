using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.PartialCruds
{
    public class GetPartialCrudByIdQuery
    {
        public Guid Id { get; set; }

        public static GetPartialCrudByIdQuery Create(Guid id)
        {
            return new GetPartialCrudByIdQuery
            {
                Id = id
            };
        }
    }
}