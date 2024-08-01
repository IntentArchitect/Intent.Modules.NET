using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandInterface", Version = "1.0")]

namespace BugSnagTest.AspNetCore.Application.Common.Interfaces
{
    public interface ICommand
    {

    }
}