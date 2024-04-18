using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Validation;
using AzureFunctions.TestApplication.Application.Validation.InboundQueVal;
using AzureFunctions.TestApplication.Domain.Common.Exceptions;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.TestApplication.Api.Validation
{
    public class InboundQueVal
    {
        private readonly IMediator _mediator;

        public InboundQueVal(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [FunctionName("Validation_InboundQueVal")]
        [OpenApiOperation("InboundQueValQuery", tags: new[] { "Validation" }, Description = "Inbound que val query")]
        [OpenApiParameter(name: "rangeStr", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "minStr", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "maxStr", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "rangeInt", In = ParameterLocation.Query, Required = true, Type = typeof(int))]
        [OpenApiParameter(name: "minInt", In = ParameterLocation.Query, Required = true, Type = typeof(int))]
        [OpenApiParameter(name: "maxInt", In = ParameterLocation.Query, Required = true, Type = typeof(int))]
        [OpenApiParameter(name: "isRequired", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "isRequiredEmpty", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "decimalRange", In = ParameterLocation.Query, Required = true, Type = typeof(decimal))]
        [OpenApiParameter(name: "decimalMin", In = ParameterLocation.Query, Required = true, Type = typeof(decimal))]
        [OpenApiParameter(name: "decimalMax", In = ParameterLocation.Query, Required = true, Type = typeof(decimal))]
        [OpenApiParameter(name: "stringOption", In = ParameterLocation.Query, Required = false, Type = typeof(string))]
        [OpenApiParameter(name: "stringOptionNonEmpty", In = ParameterLocation.Query, Required = false, Type = typeof(string))]
        [OpenApiParameter(name: "myEnum", In = ParameterLocation.Query, Required = true, Type = typeof(EnumDescriptions))]
        [OpenApiParameter(name: "regexField", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DummyResultDto))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "validation/inbound-validation")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                string rangeStr = req.Query["rangeStr"];
                string minStr = req.Query["minStr"];
                string maxStr = req.Query["maxStr"];
                int rangeInt = AzureFunctionHelper.GetQueryParam("rangeInt", req.Query, (string val, out int parsed) => int.TryParse(val, out parsed));
                int minInt = AzureFunctionHelper.GetQueryParam("minInt", req.Query, (string val, out int parsed) => int.TryParse(val, out parsed));
                int maxInt = AzureFunctionHelper.GetQueryParam("maxInt", req.Query, (string val, out int parsed) => int.TryParse(val, out parsed));
                string isRequired = req.Query["isRequired"];
                string isRequiredEmpty = req.Query["isRequiredEmpty"];
                decimal decimalRange = AzureFunctionHelper.GetQueryParam("decimalRange", req.Query, (string val, out decimal parsed) => decimal.TryParse(val, out parsed));
                decimal decimalMin = AzureFunctionHelper.GetQueryParam("decimalMin", req.Query, (string val, out decimal parsed) => decimal.TryParse(val, out parsed));
                decimal decimalMax = AzureFunctionHelper.GetQueryParam("decimalMax", req.Query, (string val, out decimal parsed) => decimal.TryParse(val, out parsed));
                string stringOption = req.Query["stringOption"];
                string stringOptionNonEmpty = req.Query["stringOptionNonEmpty"];
                EnumDescriptions myEnum = AzureFunctionHelper.GetQueryParam("myEnum", req.Query, (string val, out EnumDescriptions parsed) => EnumDescriptions.TryParse(val, out parsed));
                string regexField = req.Query["regexField"];
                var result = await _mediator.Send(new InboundQueValQuery(rangeStr: rangeStr, minStr: minStr, maxStr: maxStr, rangeInt: rangeInt, minInt: minInt, maxInt: maxInt, isRequired: isRequired, isRequiredEmpty: isRequiredEmpty, decimalRange: decimalRange, decimalMin: decimalMin, decimalMax: decimalMax, stringOption: stringOption, stringOptionNonEmpty: stringOptionNonEmpty, myEnum: myEnum, regexField: regexField), cancellationToken);
                return result != null ? new OkObjectResult(result) : new NotFoundResult();
            }
            catch (NotFoundException exception)
            {
                return new NotFoundObjectResult(new { Message = exception.Message });
            }
            catch (FormatException exception)
            {
                return new BadRequestObjectResult(new { Message = exception.Message });
            }
        }
    }
}