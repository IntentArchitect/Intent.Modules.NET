using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.Annotations.APIGateway;
using AwsLambdaFunction.Application.Common.Exceptions;
using AwsLambdaFunction.Domain.Common.Exceptions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Lambda.Functions.ExceptionHandlerHelper", Version = "1.0")]

namespace AwsLambdaFunction.Api.Helpers
{
    public static class ExceptionHandlerHelper
    {
        public static async Task<IHttpResult> ExecuteAsync(Func<Task<IHttpResult>> operation)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private static IHttpResult HandleException(Exception exception)
        {
            switch (exception)
            {
                case ValidationException validationEx:
                    var errors = new List<ValidationError>();

                    foreach (var error in validationEx.Errors)
                    {
                        errors.Add(new ValidationError(error.PropertyName, error.ErrorMessage, error.ErrorCode));
                    }
                    return HttpResults.BadRequest(new ResponseErrors("ValidationError", "One or more validation errors occurred", 400, errors));
                case ForbiddenAccessException forbiddenEx:
                    return HttpResults.NewResult(HttpStatusCode.Forbidden, new ResponseDetail("Forbidden", "Access forbidden", 403, forbiddenEx.Message ?? "You do not have permission to access this resource"));
                case UnauthorizedAccessException unauthorizedEx:
                    return HttpResults.NewResult(HttpStatusCode.Unauthorized, new ResponseDetail("Unauthorized", "Authentication required", 401, unauthorizedEx.Message ?? "Authentication is required to access this resource"));
                case NotFoundException notFoundEx:
                    return HttpResults.NotFound(new ResponseDetail("NotFound", "Resource not found", 404, notFoundEx.Message ?? "The requested resource was not found"));
                default:
                    return HttpResults.NewResult(HttpStatusCode.InternalServerError, new ResponseDetail("InternalServerError", "An error occurred while processing your request", 500, "Please try again later or contact support if the problem persists"));
            }
        }

        private record ResponseDetail(string Type, string Title, int Status, object Detail);

        private record ResponseErrors(string Type, string Title, int Status, List<ValidationError> Errors);

        private record ValidationError(string Property, string Message, string Code);

    }
}