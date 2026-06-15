using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using Wolverine;
using Wolverine.CQRS.TestApplication.Application.Common.Interfaces;
using Wolverine.CQRS.TestApplication.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.UnitOfWorkMiddleware", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Application.Common.Behaviours
{
    public class UnitOfWorkMiddleware
    {
        private readonly string _exampleParam;

        public UnitOfWorkMiddleware(string exampleParam)
        {
            _exampleParam = exampleParam;
        }
    }
}

