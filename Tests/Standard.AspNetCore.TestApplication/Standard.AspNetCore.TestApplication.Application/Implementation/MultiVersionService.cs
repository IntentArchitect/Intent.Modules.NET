using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class MultiVersionService : IMultiVersionService
    {
        [IntentManaged(Mode.Merge)]
        public MultiVersionService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task OperationForVersionOne(CancellationToken cancellationToken = default)
        {

        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task OperationForVersionTwo(CancellationToken cancellationToken = default)
        {

        }
    }
}