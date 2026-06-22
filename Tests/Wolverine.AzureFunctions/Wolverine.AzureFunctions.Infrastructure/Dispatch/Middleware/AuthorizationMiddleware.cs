using System;
using System.Linq;
using System.Reflection;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.AuthorizationMiddleware", Version = "1.0")]

namespace Wolverine.AzureFunctions.Infrastructure.Dispatch.Middleware
{
    public class AuthorizationMiddleware
    {
    }
}