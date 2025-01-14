using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.CheckNewCompChildCruds
{
    public class GetCNCCChildrenQuery
    {
        public Guid CheckNewCompChildCrudId { get; set; }

        public static GetCNCCChildrenQuery Create(Guid checkNewCompChildCrudId)
        {
            return new GetCNCCChildrenQuery
            {
                CheckNewCompChildCrudId = checkNewCompChildCrudId
            };
        }
    }
}