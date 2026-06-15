using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Wolverine;
using Wolverine.CQRS.TestApplication.Application.Common.Interfaces;
using Wolverine.CQRS.TestApplication.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.ValidationMiddleware", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Application.Common.Behaviours
{
    public class ValidationMiddleware
    {
        private readonly string _exampleParam;

        public ValidationMiddleware(string exampleParam)
        {
            _exampleParam = exampleParam;
        }
    }
}
