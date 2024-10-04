using System;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.Hangfire.HangfireJobs", Version = "1.0")]

namespace Hangfire.Tests.Api.HangfireJobs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class FireAndForget
    {
        [IntentManaged(Mode.Merge)]
        public FireAndForget()
        {
        }

        [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task ExecuteAsync()
        {
            // TODO: Implement job functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}