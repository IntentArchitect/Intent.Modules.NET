using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wolverine;
using Wolverine.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.UnhandledExceptionMiddleware", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Application.Common.Behaviours
{
    public class UnhandledExceptionMiddleware
    {
        private readonly string _exampleParam;

        public UnhandledExceptionMiddleware(string exampleParam)
        {
            _exampleParam = exampleParam;
        }
    }
}
