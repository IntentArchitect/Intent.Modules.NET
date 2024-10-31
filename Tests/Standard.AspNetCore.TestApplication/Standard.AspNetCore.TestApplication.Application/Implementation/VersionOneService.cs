using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.Interfaces;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class VersionOneService : IVersionOneService
    {
        public const string ReferenceNumber = "refnumber_1234";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public VersionOneService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task OperationForVersionOne(string param, CancellationToken cancellationToken = default)
        {
            Assert.Equal(ReferenceNumber, param);
        }
    }
}