using System.Net;
using System.Text.Json;
using System.Transactions;
using AzureFunctions.NET8.Application.Customers.UpdateCustomer;
using AzureFunctions.NET8.Domain.Common.Exceptions;
using AzureFunctions.NET8.Domain.Common.Interfaces;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET8.Api.Customers
{
    public class UpdateCustomer
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCustomer(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("Customers_UpdateCustomer")]
        [OpenApiOperation("UpdateCustomerCommand", tags: new[] { "Customers" }, Description = "Update customer command")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UpdateCustomerCommand))]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "customers/{id}")] HttpRequest req,
            Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                var command = await AzureFunctionHelper.DeserializeJsonContentAsync<UpdateCustomerCommand>(req.Body, cancellationToken);
                if (id != command.Id)
                {
                    return new BadRequestObjectResult(new { Message = "Supplied 'id' does not match 'Id' from body." });
                }
                await _mediator.Send(command, cancellationToken);
                return new NoContentResult();
            }
            catch (ValidationException exception)
            {
                return new BadRequestObjectResult(exception.Errors);
            }
            catch (NotFoundException exception)
            {
                return new NotFoundObjectResult(new { exception.Message });
            }
            catch (JsonException exception)
            {
                return new BadRequestObjectResult(new { exception.Message });
            }
            catch (FormatException exception)
            {
                return new BadRequestObjectResult(new { exception.Message });
            }
        }
    }
}