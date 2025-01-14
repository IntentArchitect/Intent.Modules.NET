using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.CheckNewCompChildCruds
{
    public class GetCheckNewCompChildCrudByIdQuery
    {
        public Guid Id { get; set; }

        public static GetCheckNewCompChildCrudByIdQuery Create(Guid id)
        {
            return new GetCheckNewCompChildCrudByIdQuery
            {
                Id = id
            };
        }
    }
}