using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandInterface", Version = "1.0")]

namespace MongoDb.MultiTenancy.SeperateDb.Application.Common.Interfaces
{
    public interface ICommand
    {

    }
}