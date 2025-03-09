using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Grpc.Core;
using GrpcServer.Api.Protos.Messages;
using GrpcServer.Api.Protos.Services.TypeTestingServices;
using GrpcServer.Application;
using GrpcServer.Application.Common.Eventing;
using GrpcServer.Application.Common.Pagination;
using GrpcServer.Application.Common.Validation;
using GrpcServer.Application.Interfaces.TypeTestingServices;
using GrpcServer.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.TraditionalService", Version = "1.0")]

namespace GrpcServer.Api.Services.TypeTestingServices
{
    [Authorize]
    public class ForPagedResult : Protos.Services.TypeTestingServices.ForPagedResult.ForPagedResultBase
    {
        private readonly IForPagedResultService _appService;
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWork _unitOfWork;

        public ForPagedResult(IForPagedResultService appService, IEventBus eventBus, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public override async Task<PagedResultOfComplexTypeDto> Operation(
            ForPagedResultOperationRequest request,
            ServerCallContext context)
        {
            Application.Common.Pagination.PagedResult<Application.ComplexTypeDto> result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.Operation(request.Param.ToContract(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return PagedResultOfComplexTypeDto.Create(result);
        }

        public override async Task<ListOfPagedResultOfComplexTypeDto> OperationCollection(
            ForPagedResultOperationCollectionRequest request,
            ServerCallContext context)
        {
            List<Application.Common.Pagination.PagedResult<Application.ComplexTypeDto>> result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.OperationCollection(request.Param.Select(x => x.ToContract()).ToList(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return ListOfPagedResultOfComplexTypeDto.Create(result);
        }

        public override async Task<PagedResultOfComplexTypeDto?> OperationNullable(
            ForPagedResultOperationNullableRequest request,
            ServerCallContext context)
        {
            Application.Common.Pagination.PagedResult<Application.ComplexTypeDto> result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.OperationNullable(request.Param?.ToContract(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return PagedResultOfComplexTypeDto.Create(result);
        }

        public override async Task<ListOfPagedResultOfComplexTypeDto?> OperationNullableCollection(
            ForPagedResultOperationNullableCollectionRequest request,
            ServerCallContext context)
        {
            List<Application.Common.Pagination.PagedResult<Application.ComplexTypeDto>> result;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.OperationNullableCollection(request.Param?.Items.Select(x => x.ToContract()).ToList(), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(context.CancellationToken);
            return ListOfPagedResultOfComplexTypeDto.Create(result);
        }
    }
}