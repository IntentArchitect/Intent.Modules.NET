using Hangfire.Dashboard;
using Hangfire.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.Hangfire.HangfireDashboardAuthFilter", Version = "1.0")]

namespace Hangfire.Tests.Api.Filters
{
    public class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
    {
        public HangfireDashboardAuthFilter()
        {
        }

        [IntentManaged(Mode.Ignore)]
        public bool Authorize(DashboardContext context)
        {
            var currentUser = context.GetHttpContext().RequestServices.GetRequiredService<ICurrentUserService>();
            return currentUser.UserId is not null;
        }
    }
}