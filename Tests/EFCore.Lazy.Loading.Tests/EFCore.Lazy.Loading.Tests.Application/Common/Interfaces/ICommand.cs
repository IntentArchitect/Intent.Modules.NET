using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandInterface", Version = "1.0")]

namespace EFCore.Lazy.Loading.Tests.Application.Common.Interfaces
{
    public interface ICommand
    {

    }
}