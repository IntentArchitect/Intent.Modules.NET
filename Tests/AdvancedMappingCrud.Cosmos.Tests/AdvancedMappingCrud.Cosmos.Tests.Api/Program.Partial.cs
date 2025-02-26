using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.ProgramPartial", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Api
{
    // Expose the Program class as public for use cases such as WebApplicationFactories
    // to reference when Top Level Statements are used.
    public partial class Program
    {
    }
}