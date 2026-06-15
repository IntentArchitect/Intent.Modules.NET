using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Wolverine;
using Wolverine.CQRS.TestApplication.Application.Common.Exceptions;
using Wolverine.CQRS.TestApplication.Application.Common.Interfaces;
using Wolverine.CQRS.TestApplication.Application.Common.Security;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.AuthorizationMiddleware", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Application.Common.Behaviours
{
    public class AuthorizationMiddleware
    {
        private readonly string _exampleParam;

        public AuthorizationMiddleware(string exampleParam)
        {
            _exampleParam = exampleParam;
        }
    }
}
