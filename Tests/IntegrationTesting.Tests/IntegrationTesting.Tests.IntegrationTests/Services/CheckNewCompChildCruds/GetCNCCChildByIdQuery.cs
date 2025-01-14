using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.CheckNewCompChildCruds
{
    public class GetCNCCChildByIdQuery
    {
        public Guid CheckNewCompChildCrudId { get; set; }
        public Guid Id { get; set; }

        public static GetCNCCChildByIdQuery Create(Guid checkNewCompChildCrudId, Guid id)
        {
            return new GetCNCCChildByIdQuery
            {
                CheckNewCompChildCrudId = checkNewCompChildCrudId,
                Id = id
            };
        }
    }
}