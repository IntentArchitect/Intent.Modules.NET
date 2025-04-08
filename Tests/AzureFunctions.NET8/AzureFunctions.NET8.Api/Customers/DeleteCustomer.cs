using System.Net;
using System.Transactions;
using AzureFunctions.NET8.Application.Customers.DeleteCustomer;
using AzureFunctions.NET8.Domain.Common.Exceptions;
using AzureFunctions.NET8.Domain.Common.Interfaces;
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
    public class DeleteCustomer
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCustomer(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("Customers_DeleteCustomer")]
        [OpenApiOperation("DeleteCustomerCommand", tags: new[] { "Customers" }, Description = "Delete customer command")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "customers/{id}")] HttpRequest req,
            Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(new DeleteCustomerCommand(id: id), cancellationToken);
                return new OkResult();
            }
            catch (NotFoundException exception)
            {
                return new NotFoundObjectResult(new { exception.Message });
            }
            catch (FormatException exception)
            {
                return new BadRequestObjectResult(new { exception.Message });
            }
        }
    }
}