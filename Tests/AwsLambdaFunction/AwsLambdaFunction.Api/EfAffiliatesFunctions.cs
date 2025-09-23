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
using AwsLambdaFunction.Application.EfAffiliates;
using AwsLambdaFunction.Application.Interfaces;
using AwsLambdaFunction.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Lambda.Functions.LambdaFunctionClassTemplate", Version = "1.0")]

namespace AwsLambdaFunction.Api
{
    public class EfAffiliatesFunctions
    {
        private readonly IEfAffiliatesService _appService;
        private readonly IValidationService _validationService;
        private readonly IDynamoDBUnitOfWork _dynamoDBUnitOfWork;
        private readonly IUnitOfWork _unitOfWork;

        public EfAffiliatesFunctions(IEfAffiliatesService appService,
            IValidationService validationService,
            IDynamoDBUnitOfWork dynamoDBUnitOfWork,
            IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _dynamoDBUnitOfWork = dynamoDBUnitOfWork ?? throw new ArgumentNullException(nameof(dynamoDBUnitOfWork));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Post, "/api/ef-affiliates")]
        public async Task<IHttpResult> CreateEfAffiliateAsync([FromBody] CreateEfAffiliateDto dto)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                await _validationService.Handle(dto, cancellationToken);
                var result = Guid.Empty;

                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    result = await _appService.CreateEfAffiliate(dto, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                }

                await _dynamoDBUnitOfWork.SaveChangesAsync(cancellationToken);
                return HttpResults.Created($"/api/ef-affiliates/{Uri.EscapeDataString(result.ToString())}", new JsonResponse<Guid>(result));
            });
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Put, "/api/ef-affiliates/{id}")]
        public async Task<IHttpResult> UpdateEfAffiliateAsync(string id, [FromBody] UpdateEfAffiliateDto dto)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                await _validationService.Handle(dto, cancellationToken);
                // AWS Lambda Function Annotations have issue accepting Guid parameter types due to how string is converted to Guid.
                // Workaround by accepting string parameters and converting to Guid here.
                if (!Guid.TryParse(id, out var idGuid))
                {
                    return HttpResults.BadRequest($"Invalid format for id: {id}");
                }

                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _appService.UpdateEfAffiliate(idGuid, dto, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                }

                await _dynamoDBUnitOfWork.SaveChangesAsync(cancellationToken);
                return HttpResults.NewResult(HttpStatusCode.NoContent);
            });
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get, "/api/ef-affiliates/{id}")]
        public async Task<IHttpResult> FindEfAffiliateByIdAsync(string id)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                // AWS Lambda Function Annotations have issue accepting Guid parameter types due to how string is converted to Guid.
                // Workaround by accepting string parameters and converting to Guid here.
                if (!Guid.TryParse(id, out var idGuid))
                {
                    return HttpResults.BadRequest($"Invalid format for id: {id}");
                }
                var result = default(EfAffiliateDto);
                result = await _appService.FindEfAffiliateById(idGuid, cancellationToken);
                return result == null ? HttpResults.NotFound() : HttpResults.Ok(result);
            });
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get, "/api/ef-affiliates")]
        public async Task<IHttpResult> FindEfAffiliatesAsync()
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                var result = default(List<EfAffiliateDto>);
                result = await _appService.FindEfAffiliates(cancellationToken);
                return HttpResults.Ok(result);
            });
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Delete, "/api/ef-affiliates/{id}")]
        public async Task<IHttpResult> DeleteEfAffiliateAsync(string id)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                // AWS Lambda Function Annotations have issue accepting Guid parameter types due to how string is converted to Guid.
                // Workaround by accepting string parameters and converting to Guid here.
                if (!Guid.TryParse(id, out var idGuid))
                {
                    return HttpResults.BadRequest($"Invalid format for id: {id}");
                }

                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _appService.DeleteEfAffiliate(idGuid, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                }

                await _dynamoDBUnitOfWork.SaveChangesAsync(cancellationToken);
                return HttpResults.Ok();
            });
        }
    }
}