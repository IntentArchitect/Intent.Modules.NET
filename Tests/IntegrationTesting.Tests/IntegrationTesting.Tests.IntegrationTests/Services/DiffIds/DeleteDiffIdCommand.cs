using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.DiffIds
{
    public class DeleteDiffIdCommand
    {
        public Guid MyId { get; set; }

        public static DeleteDiffIdCommand Create(Guid myId)
        {
            return new DeleteDiffIdCommand
            {
                MyId = myId
            };
        }
    }
}