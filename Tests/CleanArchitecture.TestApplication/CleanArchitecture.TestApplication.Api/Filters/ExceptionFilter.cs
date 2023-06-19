using System;
using System.Diagnostics;
using CleanArchitecture.TestApplication.Application.Common.Exceptions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

[assembly: IntentTemplate("Intent.AspNetCore.Controllers.ExceptionFilter", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace CleanArchitecture.TestApplication.Api.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case NotFoundException ex:
                context.Result = BuildResult(
                    context,
                    problemDetails => new NotFoundObjectResult(problemDetails),
                    new ProblemDetails
                    {
                        Detail = ex.Message
                    });
                break;
            case ValidationException ex:
                foreach (var error in ex.Errors)
                {
                    context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                context.Result = BuildResult(
                    context,
                    problemDetails => new BadRequestObjectResult(problemDetails),
                    new ValidationProblemDetails(context.ModelState));
                break;
            case ForbiddenAccessException:
                context.Result = new ForbidResult();
                break;
        }
    }

    private static IActionResult BuildResult(
            ExceptionContext context,
            Func<ProblemDetails, ObjectResult> objectResultFactory,
            ProblemDetails problemDetails)
    {
        var result = objectResultFactory(problemDetails);
        problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);
        problemDetails.Type = $"https://httpstatuses.io/{result.StatusCode}";
        return result;
    }
}