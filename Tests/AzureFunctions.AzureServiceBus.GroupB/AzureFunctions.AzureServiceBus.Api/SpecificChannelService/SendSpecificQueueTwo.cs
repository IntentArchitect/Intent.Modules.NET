using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AzureFunctions.AzureServiceBus.Application;
using AzureFunctions.AzureServiceBus.Application.Common.Eventing;
using AzureFunctions.AzureServiceBus.Application.Common.Validation;
using AzureFunctions.AzureServiceBus.Application.Interfaces;
using AzureFunctions.AzureServiceBus.Domain.Common.Exceptions;
using AzureFunctions.AzureServiceBus.Domain.Common.Interfaces;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.AzureServiceBus.Api.SpecificChannelService
{
    public class SendSpecificQueueTwo
    {
        private readonly ISpecificChannelService _appService;
        private readonly IEventBus _eventBus;
        private readonly IValidationService _validator;
        private readonly IUnitOfWork _unitOfWork;

        public SendSpecificQueueTwo(ISpecificChannelService appService,
            IEventBus eventBus,
            IValidationService validator,
            IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _eventBus = eventBus;
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("SpecificChannelService_SendSpecificQueueTwo")]
        [OpenApiOperation("SendSpecificQueueTwo", tags: new[] { "SpecificChannel" }, Description = "Send specific queue two")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(PayloadDto))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "specific-channel/send-specific-queue-two")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                var dto = await AzureFunctionHelper.DeserializeJsonContentAsync<PayloadDto>(req.Body, cancellationToken);
                await _validator.Handle(dto, cancellationToken);

                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _appService.SendSpecificQueueTwo(dto, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                    await _eventBus.FlushAllAsync(cancellationToken);
                    return new CreatedResult(string.Empty, null);
                }
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