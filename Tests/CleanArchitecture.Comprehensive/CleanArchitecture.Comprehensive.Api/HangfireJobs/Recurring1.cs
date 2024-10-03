using System;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.Hangfire.HangfireJobs", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.HangfireJobs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class Recurring1
    {
        [IntentManaged(Mode.Merge)]
        public Recurring1()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task ExecuteAsync()
        {
            // TODO: Implement job functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}