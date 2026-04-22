using System;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Quartz;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.QuartzScheduler.ScheduledJob", Version = "1.0")]

namespace WindowsServiceHost.Tests.Jobs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UnScheduledJob : IJob
    {
        [IntentManaged(Mode.Merge)]
        public UnScheduledJob()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}