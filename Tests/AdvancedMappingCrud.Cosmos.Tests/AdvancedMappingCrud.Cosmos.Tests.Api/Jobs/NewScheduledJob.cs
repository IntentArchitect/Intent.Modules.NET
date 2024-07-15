using System;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Quartz;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.QuartzScheduler.ScheduledJob", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Api.Jobs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class NewScheduledJob : IJob
    {
        [IntentManaged(Mode.Merge)]
        public NewScheduledJob()
        {
        }

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public async Task Execute(IJobExecutionContext context)
        {
            // NO-OP
        }
    }
}