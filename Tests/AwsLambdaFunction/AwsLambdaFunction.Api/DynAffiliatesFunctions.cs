using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using AwsLambdaFunction.Api.Helpers;
using AwsLambdaFunction.Api.ResponseTypes;
using AwsLambdaFunction.Application.Common.Validation;
using AwsLambdaFunction.Application.DynAffiliates;
using AwsLambdaFunction.Application.Interfaces;
using AwsLambdaFunction.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Lambda.Functions.LambdaFunctionClassTemplate", Version = "1.0")]

namespace Lambda
{
    public class DynAffiliatesFunctions
    {
        private readonly ILogger<DynAffiliatesFunctions> _logger;
        private readonly IDynAffiliatesService _appService;
        private readonly IValidationService _validationService;
        private readonly IDynamoDBUnitOfWork _dynamoDBUnitOfWork;
        private readonly IUnitOfWork _unitOfWork;

        public DynAffiliatesFunctions(ILogger<DynAffiliatesFunctions> logger,
            IDynAffiliatesService appService,
            IValidationService validationService,
            IDynamoDBUnitOfWork dynamoDBUnitOfWork,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _dynamoDBUnitOfWork = dynamoDBUnitOfWork ?? throw new ArgumentNullException(nameof(dynamoDBUnitOfWork));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Post, "/api/dyn-affiliates")]
        public async Task<IHttpResult> CreateDynAffiliateAsync([FromBody] CreateDynAffiliateDto dto)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                await _validationService.Handle(dto, cancellationToken);
                var result = default(string);

                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    result = await _appService.CreateDynAffiliate(dto, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                }

                await _dynamoDBUnitOfWork.SaveChangesAsync(cancellationToken);
                return HttpResults.Created($"/api/dyn-affiliates/{Uri.EscapeDataString(result.ToString())}", new JsonResponse<string>(result));
            }, _logger);
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Put, "/api/dyn-affiliates/{id}")]
        public async Task<IHttpResult> UpdateDynAffiliateAsync(string id, [FromBody] UpdateDynAffiliateDto dto)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                await _validationService.Handle(dto, cancellationToken);

                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _appService.UpdateDynAffiliate(id, dto, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                }

                await _dynamoDBUnitOfWork.SaveChangesAsync(cancellationToken);
                return HttpResults.NewResult(HttpStatusCode.NoContent);
            }, _logger);
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get, "/api/dyn-affiliates/{id}")]
        public async Task<IHttpResult> FindAffiliateByIdAsync(string id)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                var result = default(DynAffiliateDto);
                result = await _appService.FindAffiliateById(id, cancellationToken);
                return result == null ? HttpResults.NotFound() : HttpResults.Ok(result);
            }, _logger);
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get, "/api/dyn-affiliates")]
        public async Task<IHttpResult> FindDynAffiliatesAsync()
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                var result = default(List<DynAffiliateDto>);
                result = await _appService.FindDynAffiliates(cancellationToken);
                return HttpResults.Ok(result);
            }, _logger);
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Delete, "/api/dyn-affiliates/{id}")]
        public async Task<IHttpResult> DeleteDynAffiliateAsync(string id)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _appService.DeleteDynAffiliate(id, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                }

                await _dynamoDBUnitOfWork.SaveChangesAsync(cancellationToken);
                return HttpResults.Ok();
            }, _logger);
        }
    }
}