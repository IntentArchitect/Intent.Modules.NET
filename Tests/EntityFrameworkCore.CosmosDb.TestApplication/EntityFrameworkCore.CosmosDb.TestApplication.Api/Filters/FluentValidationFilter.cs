using FluentValidationFilter;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.FluentValidationFilter", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Api.Filters
{
    public class FluentValidationFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                context.Result = new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
            }
        }
    }
}