using Intent.RoslynWeaver.Attributes;
using Scalar.NET11.Bearer.Application.Common.Patching;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Scalar.NET11.Bearer.Application
{
    public class PatchWidgetDto
    {
        public PatchWidgetDto()
        {
        }

        public string? Name { get; set; }
        public required IPatchExecutor<PatchWidgetDto> PatchExecutor { get; init; }
    }
}