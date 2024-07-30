using System;
using System.Collections.Generic;
using System.Diagnostics;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.ExceptionFilter", Version = "1.0")]

namespace BugSnagTest.AspNetCore.Api.Filters
{
    public class ExceptionFilter : Microsoft.AspNetCore.Mvc.Filters.IExceptionFilter
    {
        public ExceptionFilter(Bugsnag.IClient client)
        {
            client.BeforeNotify(report =>
            {
                var activityId = System.Diagnostics.Activity.Current?.Id;
                if (!string.IsNullOrEmpty(activityId))
                {
                    // Add the Activity ID to the report's metadata under a 'Trace' tab
                    report.Event.Metadata.Add(
                        "Trace", new Dictionary<string, object>
                        {
                            { "ActivityId", activityId }
                        });
                }
            });
        }

        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case UnauthorizedAccessException:
                    context.Result = new UnauthorizedResult();
                    context.ExceptionHandled = true;
                    break;
            }
        }
    }

    internal static class ProblemDetailsExtensions
    {
        public static IActionResult AddContextInformation(this ObjectResult objectResult, ExceptionContext context)
        {
            if (objectResult.Value is not ProblemDetails problemDetails)
            {
                return objectResult;
            }
            problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);
            problemDetails.Type = "https://httpstatuses.io/" + (objectResult.StatusCode ?? problemDetails.Status);
            return objectResult;
        }
    }
}