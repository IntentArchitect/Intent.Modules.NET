using System;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Quartz;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.QuartzScheduler.ScheduledJob", Version = "1.0")]

namespace WindowsServiceHost.Tests.Jobs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class RunEvery15Seconds : IJob
    {
        [IntentManaged(Mode.Merge)]
        public RunEvery15Seconds()
        {
        }

        [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
        public Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}