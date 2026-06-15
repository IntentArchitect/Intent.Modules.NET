using System.Diagnostics;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wolverine;
using Wolverine.CQRS.TestApplication.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.PerformanceMiddleware", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Application.Common.Behaviours
{
    public class PerformanceMiddleware
    {
        private readonly string _exampleParam;

        public PerformanceMiddleware(string exampleParam)
        {
            _exampleParam = exampleParam;
        }
    }
}
