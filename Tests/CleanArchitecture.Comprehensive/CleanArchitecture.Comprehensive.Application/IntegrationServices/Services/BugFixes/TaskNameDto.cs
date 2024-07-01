using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.IntegrationServices.Services.BugFixes
{
    public class TaskNameDto
    {
        public static TaskNameDto Create()
        {
            return new TaskNameDto
            {
            };
        }
    }
}