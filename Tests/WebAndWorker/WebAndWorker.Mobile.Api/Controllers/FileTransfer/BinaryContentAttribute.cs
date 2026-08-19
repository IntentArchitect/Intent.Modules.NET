using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.BinaryContentAttribute", Version = "1.0")]

namespace WebAndWorker.Mobile.Api.Controllers.FileTransfer
{
    [AttributeUsage(AttributeTargets.Method)]
    public class BinaryContentAttribute : Attribute
    {
    }
}