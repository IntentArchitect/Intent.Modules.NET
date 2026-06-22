using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryInterface", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.Common.Interfaces
{
    public interface IQuery
    {
    }
}