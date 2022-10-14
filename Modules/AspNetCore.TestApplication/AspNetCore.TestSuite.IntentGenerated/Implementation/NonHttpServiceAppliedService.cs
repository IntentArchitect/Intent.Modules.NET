using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.TestSuite.IntentGenerated.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AspNetCore.TestSuite.IntentGenerated.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class NonHttpServiceAppliedService : INonHttpServiceAppliedService
    {

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public NonHttpServiceAppliedService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Operation1()
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}