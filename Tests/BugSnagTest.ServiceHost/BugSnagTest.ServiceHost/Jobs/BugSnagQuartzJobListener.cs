using System;
using System.Threading;
using System.Threading.Tasks;
using Bugsnag;
using Bugsnag.Payload;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Bugsnag.BugSnagQuartzJobListener", Version = "1.0")]

namespace BugSnagTest.ServiceHost.Jobs;

public class BugSnagQuartzJobListener : IJobListener
{
    private readonly IServiceProvider _serviceProvider;

    public BugSnagQuartzJobListener(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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

    public async Task JobWasExecuted(
            IJobExecutionContext context,
            JobExecutionException? jobException,
            CancellationToken cancellationToken = default)
    {
        await using var scopedServiceProvider = _serviceProvider.CreateAsyncScope();
        var client = scopedServiceProvider.ServiceProvider.GetRequiredService<IClient>();

        if (jobException is not null)
        {
            client.Notify(jobException, HandledState.ForUnhandledException());
        }
    }
}