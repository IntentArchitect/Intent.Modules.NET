using System.Threading;
using System.Threading.Tasks;
using Bugsnag;
using Bugsnag.Payload;
using Intent.RoslynWeaver.Attributes;
using Quartz;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Bugsnag.BugSnagQuartzJobListener", Version = "1.0")]

namespace BugSnagTest.ServiceHost.Jobs;

public class BugSnagQuartzJobListener : IJobListener
{
    private readonly IClient _client;

    public BugSnagQuartzJobListener(IClient client)
    {
        _client = client;
    }

    public string Name => "BugSnag";

    public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task JobWasExecuted(
            IJobExecutionContext context,
            JobExecutionException? jobException,
            CancellationToken cancellationToken = default)
    {
        if (jobException is not null)
        {
            _client.Notify(jobException, HandledState.ForUnhandledException());
        }

        return Task.CompletedTask;
    }
}