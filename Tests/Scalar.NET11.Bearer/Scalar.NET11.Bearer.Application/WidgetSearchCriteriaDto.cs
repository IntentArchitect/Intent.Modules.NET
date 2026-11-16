using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Scalar.NET11.Bearer.Application
{
    public class WidgetSearchCriteriaDto
    {
        public WidgetSearchCriteriaDto()
        {
        }

        public string? Name { get; set; }
    }
}