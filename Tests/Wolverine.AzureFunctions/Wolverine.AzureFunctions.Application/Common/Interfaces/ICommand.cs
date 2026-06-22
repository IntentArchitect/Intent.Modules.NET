using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandInterface", Version = "1.0")]

namespace Wolverine.AzureFunctions.Application.Common.Interfaces
{
    public interface ICommand
    {
    }
}