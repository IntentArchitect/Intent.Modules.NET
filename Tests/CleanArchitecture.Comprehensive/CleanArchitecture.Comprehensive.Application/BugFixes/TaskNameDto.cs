using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.BugFixes
{
    public class TaskNameDto
    {
        public TaskNameDto()
        {
        }

        public static TaskNameDto Create()
        {
            return new TaskNameDto
            {
            };
        }
    }
}