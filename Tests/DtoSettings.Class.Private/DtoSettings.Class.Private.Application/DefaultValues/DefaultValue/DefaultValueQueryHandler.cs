using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace DtoSettings.Class.Private.Application.DefaultValues.DefaultValue
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DefaultValueQueryHandler : IRequestHandler<DefaultValueQuery, int>
    {
        [IntentManaged(Mode.Merge)]
        public DefaultValueQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<int> Handle(DefaultValueQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (DefaultValueQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}