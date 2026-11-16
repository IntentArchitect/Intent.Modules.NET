using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace SwashbuckleSettings.NET11.Application.Widgets
{
    public class WidgetDto
    {
        public WidgetDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}