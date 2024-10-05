using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using FastEndpoints;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.ExceptionProcessorTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints
{
    public class ExceptionProcessor : IGlobalPostProcessor
    {
        public async Task PostProcessAsync(IPostProcessorContext context, CancellationToken ct)
        {
            if (!context.HasExceptionOccurred || context.HttpContext.ResponseStarted())
            {
                return;
            }

            switch (context.ExceptionDispatchInfo.SourceException)
            {
                case ValidationException exception:
                    context.MarkExceptionAsHandled();
                    await context.HttpContext.Response.SendResultAsync(new ProblemDetails(exception.Errors.ToList(), context.HttpContext.Request.Path, Activity.Current?.Id ?? context.HttpContext.TraceIdentifier, StatusCodes.Status400BadRequest));
                    break;
                case Application.Common.Exceptions.ForbiddenAccessException:
                    context.MarkExceptionAsHandled();
                    await context.HttpContext.Response.SendResultAsync(Results.Forbid());
                    break;
                case UnauthorizedAccessException:
                    context.MarkExceptionAsHandled();
                    await context.HttpContext.Response.SendResultAsync(Results.Unauthorized());
                    break;
                case NotFoundException exception:
                    context.MarkExceptionAsHandled();
                    context.HttpContext.Response.HttpContext.MarkResponseStart();
                    await context.HttpContext.Response.SendResultAsync(Results.NotFound(new { Detail = exception.Message, TraceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier }));
                    break;
                default:
                    context.ExceptionDispatchInfo.Throw();
                    break;
            }
        }
    }
}