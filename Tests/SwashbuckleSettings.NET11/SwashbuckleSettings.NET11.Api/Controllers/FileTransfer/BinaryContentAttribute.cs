using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.BinaryContentAttribute", Version = "1.0")]

namespace SwashbuckleSettings.NET11.Api.Controllers.FileTransfer
{
    [AttributeUsage(AttributeTargets.Method)]
    public class BinaryContentAttribute : Attribute
    {
    }
}