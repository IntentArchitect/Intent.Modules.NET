using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using AzureFunctions.AzureEventGrid.Domain.Common.Exceptions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.Isolated.GlobalExceptionMiddlewareTemplate", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Api
{
    public class GlobalExceptionMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ve)
            {
                await WriteJsonError(context, HttpStatusCode.BadRequest, ve.ValidationResult?.ErrorMessage ?? ve.Message);
            }
            catch (NotFoundException nf)
            {
                await WriteJsonError(context, HttpStatusCode.NotFound, nf.Message);
            }
            catch (JsonException je)
            {
                await WriteJsonError(context, HttpStatusCode.BadRequest, je.Message);
            }
            catch (FormatException fe)
            {
                await WriteJsonError(context, HttpStatusCode.BadRequest, fe.Message);
            }
            catch (Exception ex)
            {
                await WriteJsonError(context, HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private static async Task WriteJsonError(FunctionContext context, HttpStatusCode statusCode, string message)
        {
            var req = await context.GetHttpRequestDataAsync();

            if (req is null)
            {
                return;
            }
            var response = req.CreateResponse(statusCode);
            var error = new { error = message };
            await response.WriteAsJsonAsync(error);
            context.GetInvocationResult().Value = response;
        }
    }
}